namespace CREInterpreter;

using System.Collections.Generic;

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