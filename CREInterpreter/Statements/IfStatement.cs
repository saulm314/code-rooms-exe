using CREInterpreter.Tokens;
using System;
using System.Collections.Generic;

namespace CREInterpreter.Statements;

public class IfStatement(ReadOnlyMemory<char> chunkText, ReadOnlyMemory<IToken> tokens, ReadOnlyMemory<IToken> expressionTokens)
    : Statement(chunkText, tokens)
{
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