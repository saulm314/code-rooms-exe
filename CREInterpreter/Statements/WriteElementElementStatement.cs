using CREInterpreter.Tokens;
using System.Collections.Generic;

namespace CREInterpreter.Statements;

public class WriteElementElementStatement(IToken[] tokens, string chunkText, string variableName,
    IToken[] element1ExpressionTokens, IToken[] element2ExpressionTokens, IToken[] expressionTokens)
    : Statement
{
    public override IToken[] Tokens => tokens;

    public override string ChunkText => chunkText;

    public string VariableName => variableName;

    public IToken[] Element1ExpressionTokens => element1ExpressionTokens;

    public IToken[] Element2ExpressionTokens => element2ExpressionTokens;

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