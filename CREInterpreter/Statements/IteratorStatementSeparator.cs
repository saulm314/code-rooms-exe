using CREInterpreter.Tokens;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace CREInterpreter.Statements;

internal static class IteratorStatementSeparator
{
    // this is used for the specific case of a for loop iterator, i.e.:
    // for (int i = 0; i < 5; i = i + 1)
    // in this case "i = i + 1" is the iterator statement
    // however notice that unlike every other statement we process, this one does not have a semicolon
    // so we process it by artificially inserting a semicolon and then removing it at the end
    // the alternative would be to change the entire statement separator to account for a missing semicolon in this specific niche case
    // so this is potato but the alternative is even more potato
    public static IStatement? GetIteratorStatement(ReadOnlyMemory<char> text, ReadOnlyMemory<IToken> tokens, int closeBracketLineNumber, int closeBracketIndex)
    {
        ReadOnlySpan<IToken> tokenSpan = tokens.Span;
        IToken[] tokensRealloc = new IToken[tokens.Length + 1];
        for (int i = 0; i < tokens.Length; i++)
            tokensRealloc[i] = tokenSpan[i];
        tokensRealloc[^1] = new SemicolonSymbolToken(ReadOnlyMemory<char>.Empty, closeBracketLineNumber, closeBracketIndex);
        IStatement[] statements = StatementSeparator.GetStatements(text, tokensRealloc).ToArray();
        if (statements.Length > 1)
            return new InvalidStatement(text, tokens, new("More statements than expected in for loop header"));
        Debug.Assert(statements.Length == 1);
        Debug.Assert(statements[0] is Statement);
        Statement statement = (Statement)statements[0];
        PropertyInfo tokensProperty = typeof(Statement).GetProperty(nameof(Statement.Tokens))!;
        tokensProperty.SetValue(statement, tokens);
        return statement;
    }
}