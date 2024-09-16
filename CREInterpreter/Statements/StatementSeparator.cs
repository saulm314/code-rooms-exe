using CREInterpreter.Tokens;
using System;
using System.Collections.Generic;

namespace CREInterpreter.Statements;

public static class StatementSeparator
{
    public static IEnumerable<IStatement> GetStatements(ReadOnlyMemory<char> text, ReadOnlyMemory<IToken> tokens)
    {
        int index = 0;
        while (index < tokens.Length)
            yield return
                GetEmptyStatement(text, tokens, ref index) ??
                GetDeclarationStatement(text, tokens, ref index) ??
                GetInvalidStatement(text, tokens, ref index);
    }

    private static IStatement? GetEmptyStatement(ReadOnlyMemory<char> text, ReadOnlyMemory<IToken> tokens, ref int index)
    {
        ReadOnlySpan<IToken> tokenSpan = tokens.Span;
        if (tokenSpan[index] is not SemicolonSymbolToken)
            return null;
        return new EmptyStatement(text, tokens[index..++index]);
    }

    private static IStatement? GetDeclarationStatement(ReadOnlyMemory<char> text, ReadOnlyMemory<IToken> tokens, ref int index)
    {
        int startIndex = index;
        ReadOnlySpan<IToken> tokenSpan = tokens.Span;
        if (tokens.Length < index + 3)
            return null;
        if (tokenSpan[index] is not TypeNameToken typeNameToken)
            return null;
        if (tokenSpan[index + 1] is not VariableNameToken variableNameToken)
            return null;
        if (tokenSpan[index + 2] is not SemicolonSymbolToken)
            return GetDeclarationInitialisationStatement(tokenSpan, ref index);
        return new DeclarationStatement(text, tokens[index..(index += 3)], typeNameToken._VarType, variableNameToken.Text);

        IStatement? GetDeclarationInitialisationStatement(ReadOnlySpan<IToken> tokenSpan, ref int index)
        {
            if (tokens.Length < index + 5)
                return null;
            if (tokenSpan[index + 2] is not EqualsSymbolToken)
                return null;
            if (!SkipPastFirstSpecificToken<SemicolonSymbolToken>(tokenSpan, ref index))
                return new InvalidStatement(text, tokens[startIndex..], new($"Declaration-initialisation statement not followed up by a semicolon"));
            return new DeclarationInitialisationStatement
            (
                text,
                tokens[startIndex..index],
                typeNameToken._VarType,
                variableNameToken.Text,
                tokens[(startIndex + 3)..(index - 1)]
            );
        }
    }

    private static IStatement? GetWriteVariableStatement(ReadOnlyMemory<char> text, ReadOnlyMemory<IToken> tokens, ref int index)
    {
        int startIndex = index;
        ReadOnlySpan<IToken> tokenSpan = tokens.Span;
        if (tokens.Length < index + 4)
            return null;
        if (tokenSpan[index] is not VariableNameToken variableNameToken)
            return null;
        if (tokenSpan[index + 1] is not EqualsSymbolToken)
            return null;
        if (!SkipPastFirstSpecificToken<SemicolonSymbolToken>(tokenSpan, ref index))
            return new InvalidStatement(text, tokens[startIndex..], new($"Write-variable statement not followed up by a semicolon"));
        return new WriteVariableStatement
        (
            text,
            tokens[startIndex..index],
            variableNameToken.Text,
            tokens[(startIndex + 2)..(index - 1)]
        );
    }

    private static InvalidStatement GetInvalidStatement(ReadOnlyMemory<char> text, ReadOnlyMemory<IToken> tokens, ref int index)
    {
        return new(text, tokens[index..++index]);
    }

    private static void GetOpenTokenCount(IToken token, ref int openTokenCount)
    {
        switch (token)
        {
            case IOpenToken:
                openTokenCount++;
                break;
            case ICloseToken:
                openTokenCount--;
                break;
        }
    }

    private static bool SkipPastFirstSpecificToken<TToken>(ReadOnlySpan<IToken> tokenSpan, ref int index) where TToken : IToken
    {
        int openTokenCount = 0;
        for (int i = index; i < tokenSpan.Length; i++)
        {
            GetOpenTokenCount(tokenSpan[i], ref openTokenCount);
            if (openTokenCount > 0)
                continue;
            if (tokenSpan[i] is not TToken)
                continue;
            index = i + 1;
            return true;
        }
        index = tokenSpan.Length;
        return false;
    }
}