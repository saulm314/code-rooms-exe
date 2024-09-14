using CREInterpreter.Tokens;
using System;
using System.Collections.Generic;

namespace CREInterpreter.Statements;

public class WriteElementElementStatement(ReadOnlyMemory<char> chunkText, ReadOnlyMemory<IToken> tokens, ReadOnlyMemory<char> variableName,
    ReadOnlyMemory<IToken> element1ExpressionTokens, ReadOnlyMemory<IToken> element2ExpressionTokens, ReadOnlyMemory<IToken> expressionTokens)
    : Statement(chunkText, tokens)
{
    public ReadOnlyMemory<char> VariableName => variableName;

    public ReadOnlyMemory<IToken> Element1ExpressionTokens => element1ExpressionTokens;

    public ReadOnlyMemory<IToken> Element2ExpressionTokens => element2ExpressionTokens;

    public ReadOnlyMemory<IToken> ExpressionTokens => expressionTokens;

    public override InterpreterException? Compile(Memory memory)
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<InterpreterException?> Run(Memory memory)
    {
        throw new NotImplementedException();
    }
}