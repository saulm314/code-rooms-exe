using CREInterpreter.Tokens;
using System;
using System.Collections.Generic;

namespace CREInterpreter.Statements;

public class ElseStatement(ReadOnlyMemory<char> chunkText, ReadOnlyMemory<IToken> tokens) : Statement(chunkText, tokens)
{
    public override InterpreterException? Compile(Memory memory)
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<InterpreterException?> Run(Memory memory)
    {
        throw new NotImplementedException();
    }
}