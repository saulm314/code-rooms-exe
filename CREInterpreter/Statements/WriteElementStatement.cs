using CREInterpreter.Tokens;
using System.Collections.Generic;

namespace CREInterpreter.Statements;

public class WriteElementStatement(IToken[] tokens, string chunkText, string variableName, IToken[] elementExpressionTokens, IToken[] expressionTokens)
    : Statement
{
    public override IToken[] Tokens => tokens;

    public override string ChunkText => chunkText;

    public string VariableName => variableName;

    public IToken[] ElementExpressionTokens => elementExpressionTokens;

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