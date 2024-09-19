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
                GetWriteStatement(text, tokens, ref index) ??
                GetIfWhileStatement(text, tokens, ref index) ??
                GetForStatement(text, tokens, ref index) ??
                GetElseStatement(text, tokens, ref index) ??
                GetBreakStatement(text, tokens, ref index) ??
                GetContinueStatement(text, tokens, ref index) ??
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

    private static IStatement? GetWriteStatement(ReadOnlyMemory<char> chunkText, ReadOnlyMemory<IToken> chunkTokens, ref int index) =>
        WriteStatementSeparator.GetWriteStatement(chunkText, chunkTokens, ref index);

    private static IStatement? GetIfWhileStatement(ReadOnlyMemory<char> chunkText, ReadOnlyMemory<IToken> chunkTokens, ref int index)
    {
        int startIndex = index;
        ReadOnlySpan<IToken> tokenSpan = chunkTokens.Span[index..];
        Func<int, IStatement> generator;
        switch (tokenSpan)
        {
            case [IfKeywordToken, OpenBracketSymbolToken, ..]:
                generator = (index) => new IfStatement(chunkText, chunkTokens[startIndex..index], chunkTokens[2..(index - 1)]);
                break;
            case [WhileKeywordToken, OpenBracketSymbolToken, ..]:
                generator = (index) => new WhileStatement(chunkText, chunkTokens[startIndex..index], chunkTokens[2..(index - 1)]);
                break;
            default:
                return null;
        }
        int offset = 2;
        SkipToFirstTopLevelSpecificToken<CloseBracketSymbolToken>(tokenSpan, ref offset);
        if (offset == -1)
        {
            index = chunkTokens.Length;
            return new InvalidStatement(chunkText, chunkTokens[startIndex..], new("Bracket never closed"));
        }
        index += offset + 1;
        return generator(index);
    }

    private static IStatement? GetForStatement(ReadOnlyMemory<char> chunkText, ReadOnlyMemory<IToken> chunkTokens, ref int index)
    {
        int startIndex = index;
        ReadOnlySpan<IToken> tokenSpan = chunkTokens.Span[index..];
        if (tokenSpan is not [ForKeywordToken, OpenBracketSymbolToken, ..])
            return null;
        int offset = 2;
        SkipToFirstTopLevelSpecificToken<SemicolonSymbolToken>(tokenSpan, ref offset);
        if (offset == -1)
        {
            offset = 2;
            SkipToFirstTopLevelSpecificToken<CloseBracketSymbolToken>(tokenSpan, ref offset);
            index = offset == -1 ? chunkTokens.Length : index + offset + 1;
            return new InvalidStatement(chunkText, chunkTokens[startIndex..index], new("Semicolon expected in for statement"));
        }
        offset++;
        (int, int) initialiserStatementBounds = (index + 2, index + offset);
        int expressionOffset = offset;
        SkipToFirstTopLevelSpecificToken<SemicolonSymbolToken>(tokenSpan, ref offset);
        if (offset == -1)
        {
            offset = expressionOffset;
            SkipToFirstTopLevelSpecificToken<CloseBracketSymbolToken>(tokenSpan, ref offset);
            index = offset == -1 ? chunkTokens.Length : index + offset + 1;
            return new InvalidStatement(chunkText, chunkTokens[startIndex..index], new("Second semicolon expected in for statement"));
        }
        offset++;
        (int, int) expressionBounds = (index + expressionOffset, index + offset - 1);
        int iteratorStatementOffset = offset;
        SkipToFirstTopLevelSpecificToken<CloseBracketSymbolToken>(tokenSpan, ref offset);
        if (offset == -1)
        {
            index = chunkTokens.Length;
            return new InvalidStatement(chunkText, chunkTokens[startIndex..index], new("Bracket is never closed"));
        }
        offset++;
        (int, int) iteratorStatementBounds = (index + iteratorStatementOffset, index + offset - 1);
        index += offset;
        return new ForStatement(chunkText, chunkTokens[startIndex..index],
            chunkTokens[initialiserStatementBounds.Item1..initialiserStatementBounds.Item2],
            chunkTokens[expressionBounds.Item1..expressionBounds.Item2],
            chunkTokens[iteratorStatementBounds.Item1..iteratorStatementBounds.Item2]);
    }

    private static IStatement? GetElseStatement(ReadOnlyMemory<char> chunkText, ReadOnlyMemory<IToken> chunkTokens, ref int index)
    {
        ReadOnlySpan<IToken> tokenSpan = chunkTokens.Span[index..];
        if (tokenSpan is not [ElseKeywordToken, ..])
            return null;
        return new ElseStatement(chunkText, chunkTokens[index..++index]);
    }

    private static IStatement? GetBreakStatement(ReadOnlyMemory<char> chunkText, ReadOnlyMemory<IToken> chunkTokens, ref int index)
    {
        ReadOnlySpan<IToken> tokenSpan = chunkTokens.Span[index..];
        if (tokenSpan is not [BreakKeywordToken, SemicolonSymbolToken, ..])
            return null;
        return new BreakStatement(chunkText, chunkTokens[index..(index += 2)]);
    }

    private static IStatement? GetContinueStatement(ReadOnlyMemory<char> chunkText, ReadOnlyMemory<IToken> chunkTokens, ref int index)
    {
        ReadOnlySpan<IToken> tokenSpan = chunkTokens.Span[index..];
        if (tokenSpan is not [ContinueKeywordToken, SemicolonSymbolToken, ..])
            return null;
        return new ContinueStatement(chunkText, chunkTokens[index..(index += 2)]);
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
            if (openTokenCount < 0)
                break;
        }
        index = -1;
    }
}