using CREInterpreter.Tokens;
using System;
using System.Collections.Generic;

namespace CREInterpreter.Statements;

public class DeclarationInitialisationStatement(ReadOnlyMemory<char> chunkText, ReadOnlyMemory<IToken> tokens, VarType varType,
    ReadOnlyMemory<char> variableName, ReadOnlyMemory<IToken> expressionTokens)
    : Statement(chunkText, tokens), IInitialiserStatement
{
    public VarType _VarType => varType;

    public ReadOnlyMemory<char> VariableName => variableName;

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