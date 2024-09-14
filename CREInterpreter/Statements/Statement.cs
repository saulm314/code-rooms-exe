using CREInterpreter.Tokens;
using System;
using System.Collections.Generic;

namespace CREInterpreter.Statements;

public abstract class Statement
{
    public Statement(ReadOnlyMemory<char> chunkText, ReadOnlyMemory<IToken> tokens)
    {
        Tokens = tokens;
        if (tokens.Length == 0)
        {
            LineNumber = -1;
            Index = -1;
            Text = ReadOnlyMemory<char>.Empty;
            return;
        }
        ReadOnlySpan<IToken> tokensSpan = tokens.Span;
        LineNumber = tokensSpan[0].LineNumber;
        Index = tokensSpan[0].Index;
        int endIndex = tokensSpan[^1].Index + tokensSpan[^1].Text.Length;
        Text = chunkText[Index..endIndex];
    }

    public ReadOnlyMemory<IToken> Tokens { get; init; }

    public ReadOnlyMemory<char> Text { get; init; }

    public int LineNumber { get; init; }

    public int Index { get; init; }

    public abstract InterpreterException? Compile(Memory memory);

    public abstract IEnumerable<InterpreterException?> Run(Memory memory);

    public override string? ToString() => Text.ToString();
}