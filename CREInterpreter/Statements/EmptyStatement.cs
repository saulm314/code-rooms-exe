using CREInterpreter.Tokens;
using System.Collections.Generic;

namespace CREInterpreter.Statements;

public class EmptyStatement(IToken[] tokens, string chunkText) : Statement
{
    public override IToken[] Tokens => tokens;

    public override string ChunkText => chunkText;

    public override InterpreterException? Compile(Memory memory)
    {
        throw new System.NotImplementedException();
    }

    public override IEnumerable<InterpreterException?> Run(Memory memory)
    {
        throw new System.NotImplementedException();
    }
}