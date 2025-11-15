using CREInterpreter.Tokens;
using System;
using System.Collections.Generic;

namespace CREInterpreter.Statements;

public class BlockStatement(ReadOnlyMemory<char> chunkText, ReadOnlyMemory<IToken> tokens, ReadOnlyMemory<IToken> blockTokens)
    : Statement(chunkText, tokens), IStatement
{
    public ReadOnlyMemory<IToken> BlockTokens => blockTokens;

    public override InterpreterException? Compile(Memory memory)
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<StatementExecution> Execute(Memory memory)
    {
        throw new NotImplementedException();
    }
}