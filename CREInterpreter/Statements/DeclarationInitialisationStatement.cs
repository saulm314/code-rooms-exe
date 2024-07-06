using CREInterpreter.Tokens;
using System.Collections.Generic;

namespace CREInterpreter.Statements;

public class DeclarationInitialisationStatement(IToken[] tokens, string chunkText, VarType varType, string variableName, IToken[] expressionTokens)
    : Statement
{
    public override IToken[] Tokens => tokens;

    public override string ChunkText => chunkText;

    public VarType _VarType => varType;

    public string VariableName => variableName;

    public IToken[] ExpressionTokens => expressionTokens;

    public override InterpreterException? Compile(Memory memory)
    {
        throw new System.NotImplementedException();
    }

    public override IEnumerable<InterpreterException?> Run(Memory memory)
    {
        throw new System.NotImplementedException();
    }
}