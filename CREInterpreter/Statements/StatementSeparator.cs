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

    private static InvalidStatement GetInvalidStatement(ReadOnlyMemory<char> text, ReadOnlyMemory<IToken> tokens, ref int index)
    {
        return new(text, tokens[index..++index]);
    }
}