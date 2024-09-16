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
        ReadOnlySpan<IToken> tokenSpan = tokens.Span;
        if (tokens.Length < index + 3)
            return null;
        if (tokenSpan[index] is not TypeNameToken typeNameToken)
            return null;
        if (tokenSpan[index + 1] is not VariableNameToken variableNameToken)
            return null;
        if (tokenSpan[index + 2] is not SemicolonSymbolToken)
            return null;
        return new DeclarationStatement(text, tokens[index..(index += 3)], typeNameToken._VarType, variableNameToken.Text);
    }

    private static InvalidStatement GetInvalidStatement(ReadOnlyMemory<char> text, ReadOnlyMemory<IToken> tokens, ref int index)
    {
        return new(text, tokens[index..++index]);
    }
}