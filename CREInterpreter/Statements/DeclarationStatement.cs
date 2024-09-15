using CREInterpreter.Tokens;
using System;
using System.Collections.Generic;

namespace CREInterpreter.Statements;

public class DeclarationStatement(ReadOnlyMemory<char> chunkText, ReadOnlyMemory<IToken> tokens, VarType varType, ReadOnlyMemory<char> variableName)
    : Statement(chunkText, tokens)
{
    public VarType _VarType => varType;

    public ReadOnlyMemory<char> VariableName => variableName;

    public override InterpreterException? Compile(Memory memory)
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<StatementExecution> Execute(Memory memory)
    {
        throw new NotImplementedException();
    }
}