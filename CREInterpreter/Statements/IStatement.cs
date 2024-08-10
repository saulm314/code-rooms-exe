using CREInterpreter.Tokens;
using System.Collections.Generic;

namespace CREInterpreter.Statements;

public interface IStatement
{
    IToken[] Tokens { get; }

    string ChunkText { get; }

    string Text =>
        _Text ??=
        Tokens.Length == 0 ? string.Empty :
        ChunkText[Tokens[0].Index..(Tokens[^1].Index + Tokens[^1].Text.Length)];

    int LineNumber =>
        _LineNumber ??=
        Tokens.Length == 0 ? -1 :
        Tokens[0].LineNumber;

    InterpreterException? Compile(Memory memory);

    IEnumerable<InterpreterException?> Run(Memory memory);

    string? ToString() => Text;

    // no need to provide manual implementations for the getters and setters here
    // _Text and _LineNumber are just here so that the implementing class can provide backing fields for them
    // so auto-implemented properties are perfectly fine
    protected string? _Text { get; set; }

    protected int? _LineNumber { get; set; }
}