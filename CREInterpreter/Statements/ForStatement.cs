using CREInterpreter.Tokens;
using System;
using System.Collections.Generic;

namespace CREInterpreter.Statements;

public class ForStatement(ReadOnlyMemory<char> chunkText, ReadOnlyMemory<IToken> tokens,
    ReadOnlyMemory<IToken> intialiserStatementTokens, ReadOnlyMemory<IToken> expressionTokens, ReadOnlyMemory<IToken> iteratorStatementTokens)
    : Statement(chunkText, tokens), IStatement
{
    public ReadOnlyMemory<IToken> InitialiserStatementTokens => intialiserStatementTokens;

    public ReadOnlyMemory<IToken> ExpressionTokens => expressionTokens;

    public ReadOnlyMemory<IToken> IteratorStatementTokens => iteratorStatementTokens;

    public override InterpreterException? Compile(Memory memory)
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<StatementExecution> Execute(Memory memory)
    {
        throw new NotImplementedException();
    }
}