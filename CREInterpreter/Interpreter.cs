using System.Collections.Generic;

namespace CREInterpreter;

public class Interpreter(string Text)
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