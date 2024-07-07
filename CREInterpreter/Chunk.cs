using System.Collections.Generic;

namespace CREInterpreter;

public class Chunk(string text)
{
    public string Text => text;

    public InterpreterException? Compile(Memory memory)
    {
        return null;
    }

    public IEnumerable<InterpreterException?> Run(Memory memory)
    {
        yield break;
    }

    public override string? ToString() => Text;
}