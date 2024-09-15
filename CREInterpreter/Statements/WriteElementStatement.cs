using CREInterpreter.Tokens;
using System;
using System.Collections.Generic;

namespace CREInterpreter.Statements;

public class WriteElementStatement(ReadOnlyMemory<char> chunkText, ReadOnlyMemory<IToken> tokens, ReadOnlyMemory<char> variableName,
    ReadOnlyMemory<IToken> elementExpressionTokens, ReadOnlyMemory<IToken> expressionTokens)
    : Statement(chunkText, tokens), IInitialiserStatement, IIteratorStatement
{
    public ReadOnlyMemory<char> VariableName => variableName;

    public ReadOnlyMemory<IToken> ElementExpressionTokens => elementExpressionTokens;

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