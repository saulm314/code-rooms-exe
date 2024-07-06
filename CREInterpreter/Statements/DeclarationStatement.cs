using CREInterpreter.Tokens;
using System.Collections.Generic;

namespace CREInterpreter.Statements;

public class DeclarationStatement(IToken[] tokens, string chunkText, VarType varType, string variableName) : Statement
{
    public override IToken[] Tokens => tokens;

    public override string ChunkText => chunkText;

    public VarType _VarType => varType;

    public string VariableName => variableName;

    public override InterpreterException? Compile(Memory memory)
    {
        throw new System.NotImplementedException();
    }

    public override IEnumerable<InterpreterException?> Run(Memory memory)
    {
        throw new System.NotImplementedException();
    }
}