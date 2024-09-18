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

    private static IStatement? GetEmptyStatement(ReadOnlyMemory<char> chunkText, ReadOnlyMemory<IToken> chunkTokens, ref int index)
    {
        ReadOnlySpan<IToken> tokenSpan = chunkTokens.Span[index..];
        if (tokenSpan[0] is not SemicolonSymbolToken)
            return null;
        return new EmptyStatement(chunkText, chunkTokens[index..++index]);
    }

    private static IStatement? GetDeclarationStatement(ReadOnlyMemory<char> chunkText, ReadOnlyMemory<IToken> chunkTokens, ref int index) =>
        DeclarationStatementSeparator.GetDeclarationStatement(chunkText, chunkTokens, ref index);

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
        //if (!SkipToFirstTopLevelSpecificToken<SemicolonSymbolToken>(tokenSpan, ref index))
        //    return new InvalidStatement(text, tokens[startIndex..], new($"Write-variable statement not followed up by a semicolon"));
        return new WriteVariableStatement
        (
            text,
            tokens[startIndex..index],
            variableNameToken.Text,
            tokens[(startIndex + 2)..(index - 1)]
        );
    }

    private static InvalidStatement GetInvalidStatement(ReadOnlyMemory<char> chunkText, ReadOnlyMemory<IToken> chunkTokens, ref int index)
    {
        int startIndex = index;
        SkipToFirstTopLevelSpecificToken<SemicolonSymbolToken>(chunkTokens.Span, ref index);
        if (index == -1)
        {
            index = chunkTokens.Length;
            return new(chunkText, chunkTokens[startIndex..]);
        }
        index++;
        return new(chunkText, chunkTokens[startIndex..index]);
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

    // Given a span, a starting index, and a token type,
    // the function sets the index to correspond to the first element (at or after the starting index) with the specific token type,
    // with the further criterion that the token must be top-level,
    // i.e. it must not be inside any pairs of brackets/braces that opened at or after the starting index
    // alternatively, if no such element exists, then index is set to -1
    // e.g. if tokenSpan forms the text "for (int i = 0; i < 5; i = i + 1);", then if the starting index is 0 and TToken is SemicolonSymbolToken,
    //      then the position of the final semicolon will be returned, since the first two semicolons are in parentheses
    // on the other hand if the starting index is 2 (i.e. corresponding to "int"), then the first semicolon will be returned,
    //      since the bracket was opened before the starting index
    internal static void SkipToFirstTopLevelSpecificToken<TToken>(ReadOnlySpan<IToken> tokenSpan, ref int index) where TToken : IToken
    {
        int openTokenCount = 0;
        for (int i = index; i < tokenSpan.Length; i++)
        {
            if (openTokenCount == 0 && tokenSpan[i] is TToken)
            {
                index = i;
                return;
            }
            GetOpenTokenCount(tokenSpan[i], ref openTokenCount);
        }
        index = -1;
    }
}