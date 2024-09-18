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

    private static IStatement? GetDeclarationStatement(ReadOnlyMemory<char> chunkText, ReadOnlyMemory<IToken> chunkTokens, ref int index)
    {
        int startIndex = index;
        ReadOnlySpan<IToken> tokenSpan = chunkTokens.Span[index..];
        return tokenSpan switch
        {
            [TypeNameToken typeNameToken, VariableNameToken variableNameToken, SemicolonSymbolToken, ..] =>
                new DeclarationStatement(chunkText, chunkTokens[index..(index += 3)], typeNameToken._VarType, variableNameToken.Text),

            [TypeNameToken typeNameToken, OpenSquareBraceSymbolToken, CloseSquareBraceSymbolToken, VariableNameToken variableNameToken,
                SemicolonSymbolToken, ..] =>
                new DeclarationStatement(chunkText, chunkTokens[index..(index += 5)], typeNameToken._VarType.Array!, variableNameToken.Text),

            [TypeNameToken typeNameToken, VariableNameToken variableNameToken, EqualsSymbolToken, ..] =>
                GetDeclarationInitialisationStatement(tokenSpan[3..], startIndex + 3, ref index, typeNameToken._VarType, variableNameToken.Text),

            [TypeNameToken typeNameToken, OpenSquareBraceSymbolToken, CloseSquareBraceSymbolToken, VariableNameToken variableNameToken,
                EqualsSymbolToken, ..] =>
                GetDeclarationInitialisationStatement(tokenSpan[5..], startIndex + 5, ref index, typeNameToken._VarType.Array!, variableNameToken.Text),

            _ => null
        };

        IStatement? GetDeclarationInitialisationStatement(ReadOnlySpan<IToken> expressionTokenSpan, int expressionTokenSpanIndex, ref int index,
            VarType varType, ReadOnlyMemory<char> variableName)
        {
            int offset = 0;
            SkipToFirstTopLevelSpecificToken<SemicolonSymbolToken>(expressionTokenSpan, ref offset);
            if (offset == -1)
            {
                index = chunkTokens.Length;
                return new InvalidStatement(chunkText, chunkTokens[startIndex..], new("Declaration-initialisation statement never followed by a semicolon"));
            }
            index += offset + 1;
            return new DeclarationInitialisationStatement(chunkText, chunkTokens[startIndex..index], varType, variableName,
                chunkTokens[expressionTokenSpanIndex..(index - 1)]);
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
    private static void SkipToFirstTopLevelSpecificToken<TToken>(ReadOnlySpan<IToken> tokenSpan, ref int index) where TToken : IToken
    {
        int openTokenCount = 0;
        for (int i = index; i < tokenSpan.Length; i++)
        {
            GetOpenTokenCount(tokenSpan[i], ref openTokenCount);
            if (openTokenCount > 0)
                continue;
            if (tokenSpan[i] is not TToken)
                continue;
            index = i;
            return;
        }
        index = -1;
    }
}