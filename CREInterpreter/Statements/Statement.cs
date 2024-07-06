using CREInterpreter.Tokens;
using System.Collections.Generic;

namespace CREInterpreter.Statements;

public abstract class Statement
{
    public abstract IToken[] Tokens { get; }

    public abstract string ChunkText { get; }

    public string Text =>
        _text ??=
        Tokens.Length == 0 ? string.Empty :
        ChunkText[Tokens[0].Index..(Tokens[^1].Index + Tokens[^1].Text.Length)];
    private string? _text;

    public int LineNumber =>
        _lineNumber ??=
        Tokens.Length == 0 ? -1 :
        Tokens[0].LineNumber;
    private int? _lineNumber;

    public abstract InterpreterException? Compile(Memory memory);

    public abstract IEnumerable<InterpreterException?> Run(Memory memory);

    public override string ToString() => Text;
}