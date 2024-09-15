using CREInterpreter.Tokens;
using System;
using System.Collections.Generic;

namespace CREInterpreter.Statements;

public interface IStatement
{
    ReadOnlyMemory<IToken> Tokens { get; }

    ReadOnlyMemory<char> Text { get; }

    int LineNumber { get; }

    int Index { get; }

    InterpreterException? Compile(Memory memory);

    IEnumerable<InterpreterException?> Run(Memory memory);

    string? ToString() => Text.ToString();
}