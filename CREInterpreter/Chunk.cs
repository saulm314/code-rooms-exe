using CREInterpreter.Statements;
using CREInterpreter.Tokens;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace CREInterpreter;

public class Chunk(string text)
{
    public string Text => text;

    public ImmutableArray<IToken> Tokens { get; } = TokenSeparator.GetTokens(text.AsMemory()).ToImmutableArray();

    public ImmutableArray<Statement> Statements { get; }

    public InterpreterException? Compile(Memory memory)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<InterpreterException?> Run(Memory memory)
    {
        throw new NotImplementedException();
    }

    public override string? ToString() => Text.ToString();
}