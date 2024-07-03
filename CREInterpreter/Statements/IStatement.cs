using System.Collections.Generic;

namespace CREInterpreter.Statements;

public interface IStatement
{
    string Text { get; }

    int LineNumber { get; }

    InterpreterException? Compile(Memory memory);

    IEnumerable<InterpreterException?> Run(Memory memory);

    string? ToString() => Text;
}