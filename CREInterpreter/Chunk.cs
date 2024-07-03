using System.Collections.Generic;

namespace CREInterpreter;

public class Chunk(string Text)
{
    public InterpreterException? Compile()
    {
        return null;
    }

    public IEnumerable<InterpreterException?> Run()
    {
        yield break;
    }
}