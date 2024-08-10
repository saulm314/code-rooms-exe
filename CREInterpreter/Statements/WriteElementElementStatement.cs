using CREInterpreter.Tokens;
using System.Collections.Generic;

namespace CREInterpreter.Statements;

public class WriteElementElementStatement(IToken[] tokens, string chunkText, string variableName,
    IToken[] element1ExpressionTokens, IToken[] element2ExpressionTokens, IToken[] expressionTokens)
    : IStatement
{
    public IToken[] Tokens => tokens;

    public string ChunkText => chunkText;

    public string VariableName => variableName;

    public IToken[] Element1ExpressionTokens => element1ExpressionTokens;

    public IToken[] Element2ExpressionTokens => element2ExpressionTokens;

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