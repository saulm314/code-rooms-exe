using CREInterpreter.Tokens;
using System;
using System.Collections.Generic;

namespace CREInterpreter.Statements;

public class InvalidStatement : Statement, IStatement
{
    public InvalidStatement(ReadOnlyMemory<char> chunkText, ReadOnlyMemory<IToken> tokens) : base(chunkText, tokens)
    {
        Exception = new($"Invalid statement: {Text}");
    }

    public InvalidStatement(ReadOnlyMemory<char> chunkText, ReadOnlyMemory<IToken> tokens, InterpreterException exception)
        : base(chunkText, tokens)
    {
        Exception = exception;
    }

    public InterpreterException Exception { get; init; }

    public override InterpreterException? Compile(Memory memory)
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<StatementExecution> Execute(Memory memory)
    {
        throw new NotImplementedException();
    }
}