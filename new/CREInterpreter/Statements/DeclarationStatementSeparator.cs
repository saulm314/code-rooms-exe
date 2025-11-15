using CREInterpreter.Tokens;
using System;

namespace CREInterpreter.Statements;

internal static class DeclarationStatementSeparator
{
    public static IStatement? GetDeclarationStatement(ReadOnlyMemory<char> chunkText, ReadOnlyMemory<IToken> chunkTokens, ref int index)
    {
        ReadOnlySpan<IToken> tokenSpan = chunkTokens.Span[index..];
        return tokenSpan switch
        {
            [TypeNameToken typeNameToken, VariableNameToken variableNameToken, SemicolonSymbolToken, ..] =>
                new DeclarationStatement(chunkText, chunkTokens[index..(index += 3)], typeNameToken._VarType, variableNameToken.Text),

            [TypeNameToken typeNameToken, OpenSquareBraceSymbolToken, CloseSquareBraceSymbolToken, VariableNameToken variableNameToken,
                SemicolonSymbolToken, ..] =>
                new DeclarationStatement(chunkText, chunkTokens[index..(index += 5)], typeNameToken._VarType.Array!, variableNameToken.Text),

            [TypeNameToken typeNameToken, VariableNameToken variableNameToken, EqualsSymbolToken, ..] =>
                GetDeclarationInitialisationStatement(chunkText, chunkTokens, ref index, tokenSpan[3..], index + 3,
                    typeNameToken._VarType, variableNameToken.Text),

            [TypeNameToken typeNameToken, OpenSquareBraceSymbolToken, CloseSquareBraceSymbolToken, VariableNameToken variableNameToken,
                EqualsSymbolToken, ..] =>
                GetDeclarationInitialisationStatement(chunkText, chunkTokens, ref index, tokenSpan[5..], index + 5,
                    typeNameToken._VarType.Array!, variableNameToken.Text),

            _ => null
        };
    }

    private static IStatement? GetDeclarationInitialisationStatement(ReadOnlyMemory<char> chunkText, ReadOnlyMemory<IToken> chunkTokens, ref int index,
        ReadOnlySpan<IToken> expressionTokenSpan, int expressionIndex, VarType varType, ReadOnlyMemory<char> variableName)
    {
        int startIndex = index;
        int offset = 0;
        StatementSeparator.SkipToFirstTopLevelSpecificToken<SemicolonSymbolToken>(expressionTokenSpan, ref offset);
        if (offset == -1)
        {
            index = chunkTokens.Length;
            return new InvalidStatement(chunkText, chunkTokens[startIndex..], new("Declaration-initialisation statement never followed by a semicolon"));
        }
        index += offset + 1;
        return new DeclarationInitialisationStatement(chunkText, chunkTokens[startIndex..index], varType, variableName,
            chunkTokens[expressionIndex..(index - 1)]);
    }
}