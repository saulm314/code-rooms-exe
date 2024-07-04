namespace CREInterpreter.Tokens;

public class BooleanLiteralToken(bool value, int lineNumber) : IToken
{
    public string Text => value ? "true" : "false";

    public int LineNumber => lineNumber;

    public bool Value => value;

    public InterpreterException? Compile(Memory memory)
    {
        return null;
    }
}