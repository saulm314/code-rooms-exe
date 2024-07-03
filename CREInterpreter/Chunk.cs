using System.Collections.Generic;

namespace CREInterpreter;

public class Chunk(string Text)
{
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