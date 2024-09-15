using CREInterpreter.Tokens;
using System;
using System.Collections.Generic;

namespace CREInterpreter.Statements;

public class WhileStatement(ReadOnlyMemory<char> chunkText, ReadOnlyMemory<IToken> tokens, ReadOnlyMemory<char> expressionTokens)
    : Statement(chunkText, tokens)
{
    public ReadOnlyMemory<char> ExpressionTokens => expressionTokens;

    public override InterpreterException? Compile(Memory memory)
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<StatementExecution> Execute(Memory memory)
    {
        throw new NotImplementedException();
    }
}