using CREInterpreter.Tokens;
using System;

namespace CREInterpreter.Statements;

internal static class WriteStatementSeparator
{
    public static IStatement? GetWriteStatement(ReadOnlyMemory<char> chunkText, ReadOnlyMemory<IToken> chunkTokens, ref int index)
    {
        ReadOnlySpan<IToken> tokenSpan = chunkTokens.Span[index..];
        return tokenSpan switch
        {
            [VariableNameToken variableNameToken, EqualsSymbolToken, ..] =>
                GetWriteStatement(chunkText, chunkTokens, ref index, tokenSpan[2..], index + 2, variableNameToken.Text, new WriteVariableData()),

            [VariableNameToken variableNameToken, OpenSquareBraceSymbolToken, ..] =>
                GetWriteElementStatement(chunkText, chunkTokens, ref index, tokenSpan[2..], index + 2, variableNameToken.Text),

            _ => null
        };
    }

    private interface IWriteData;

    private readonly record struct WriteVariableData() : IWriteData;

    private readonly record struct WriteElementData((int, int) ElementExpressionBounds) : IWriteData;

    private readonly record struct WriteElementElementData((int, int) Element1ExpressionBounds, (int, int) Element2ExpressionBounds) : IWriteData;

    private static IStatement? GetWriteStatement(ReadOnlyMemory<char> chunkText, ReadOnlyMemory<IToken> chunkTokens, ref int index,
        ReadOnlySpan<IToken> expressionTokenSpan, int expressionIndex, ReadOnlyMemory<char> variableName, IWriteData writeData)
    {
        int startIndex = index;
        int offset = 0;
        StatementSeparator.SkipToFirstTopLevelSpecificToken<SemicolonSymbolToken>(expressionTokenSpan, ref offset);
        if (offset == -1)
        {
            index = chunkTokens.Length;
            return new InvalidStatement(chunkText, chunkTokens[startIndex..], new("Write statement never followed by a semicolon"));
        }
        index = expressionIndex + offset + 1;
        return writeData switch
        {
            WriteVariableData data =>
                new WriteVariableStatement(chunkText, chunkTokens[startIndex..index], variableName, chunkTokens[expressionIndex..(index - 1)]),

            WriteElementData data =>
                new WriteElementStatement(chunkText, chunkTokens[startIndex..index], variableName,
                    chunkTokens[data.ElementExpressionBounds.Item1..data.ElementExpressionBounds.Item2], chunkTokens[expressionIndex..(index - 1)]),

            WriteElementElementData data =>
                new WriteElementElementStatement(chunkText, chunkTokens[startIndex..index], variableName,
                    chunkTokens[data.Element1ExpressionBounds.Item1..data.Element1ExpressionBounds.Item2],
                    chunkTokens[data.Element2ExpressionBounds.Item1..data.Element2ExpressionBounds.Item2],
                    chunkTokens[expressionIndex..(index - 1)]),

            _ => throw new InvalidOperationException("WriteStatementSeparator: unexpected IWriteData type")
        };
    }

    private static IStatement? GetWriteElementStatement(ReadOnlyMemory<char> chunkText, ReadOnlyMemory<IToken> chunkTokens, ref int index,
        ReadOnlySpan<IToken> elementExpressionSpan, int elementExpressionIndex, ReadOnlyMemory<char> variableName)
    {
        int startIndex = index;
        int offset = 0;
        StatementSeparator.SkipToFirstTopLevelSpecificToken<CloseSquareBraceSymbolToken>(elementExpressionSpan, ref offset);
        if (offset == -1)
        {
            index = chunkTokens.Length;
            return new InvalidStatement(chunkText, chunkTokens[startIndex..], new("Square brace never closed"));
        }
        (int, int) elementExpressionBounds = (elementExpressionIndex, elementExpressionIndex + offset);
        ReadOnlySpan<IToken> remainingSpan = elementExpressionSpan[(offset + 1)..];
        return remainingSpan switch
        {
            [EqualsSymbolToken, ..] =>
                GetWriteStatement(chunkText, chunkTokens, ref index, remainingSpan[1..], elementExpressionIndex + offset + 2, variableName,
                    new WriteElementData(elementExpressionBounds)),

            [OpenSquareBraceSymbolToken, ..] =>
                GetWriteElementElementStatement(chunkText, chunkTokens, ref index, remainingSpan[1..], elementExpressionIndex + offset + 2, variableName,
                    elementExpressionBounds),

            _ => null
        };
    }

    private static IStatement? GetWriteElementElementStatement(ReadOnlyMemory<char> chunkText, ReadOnlyMemory<IToken> chunkTokens, ref int index,
        ReadOnlySpan<IToken> element2ExpressionSpan, int element2ExpressionIndex, ReadOnlyMemory<char> variableName, (int, int) element1ExpressionBounds)
    {
        int startIndex = index;
        int offset = 0;
        StatementSeparator.SkipToFirstTopLevelSpecificToken<CloseSquareBraceSymbolToken>(element2ExpressionSpan, ref offset);
        if (offset == -1)
        {
            index = chunkTokens.Length;
            return new InvalidStatement(chunkText, chunkTokens[startIndex..], new("Square brace never closed"));
        }
        (int, int) element2ExpressionBounds = (element2ExpressionIndex, element2ExpressionIndex + offset);
        ReadOnlySpan<IToken> remainingSpan = element2ExpressionSpan[(offset + 1)..];
        return remainingSpan switch
        {
            [EqualsSymbolToken, ..] =>
                GetWriteStatement(chunkText, chunkTokens, ref index, remainingSpan[1..], element2ExpressionIndex + offset + 2, variableName,
                    new WriteElementElementData(element1ExpressionBounds, element2ExpressionBounds)),

            _ => null
        };
    }
}