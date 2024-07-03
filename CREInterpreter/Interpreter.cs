using System.Collections.Generic;

namespace CREInterpreter;

public class Interpreter(string Text)
{
    public InterpreterException? Compile()
    {
        return null;
    }

    public IEnumerable<InterpreterException?> Run()
    {
        yield break;
    }

    public override string? ToString() => Text;
}