using CREInterpreter.Tokens;
using System;
using System.Collections.Generic;

namespace CREInterpreter.Statements;

public class BreakStatement(ReadOnlyMemory<char> chunkText, ReadOnlyMemory<IToken> tokens) : Statement(chunkText, tokens), IStatement
{
    public override InterpreterException? Compile(Memory memory)
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<StatementExecution> Execute(Memory memory)
    {
        throw new NotImplementedException();
    }
}