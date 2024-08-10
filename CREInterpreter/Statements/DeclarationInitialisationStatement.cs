using CREInterpreter.Tokens;
using System.Collections.Generic;

namespace CREInterpreter.Statements;

public class DeclarationInitialisationStatement(IToken[] tokens, string chunkText, VarType varType, string variableName, IToken[] expressionTokens)
    : IStatement
{
    public IToken[] Tokens => tokens;

    public string ChunkText => chunkText;

    public VarType _VarType => varType;

    public string VariableName => variableName;

    public IToken[] ExpressionTokens => expressionTokens;

    public string Text => ((IStatement)this).Text;

    public int LineNumber => ((IStatement)this).LineNumber;

    public InterpreterException? Compile(Memory memory)
    {
        throw new System.NotImplementedException();
    }

    public IEnumerable<InterpreterException?> Run(Memory memory)
    {
        throw new System.NotImplementedException();
    }

    string? IStatement._Text { get; set; }

    int? IStatement._LineNumber { get; set; }
}