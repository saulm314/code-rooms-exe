namespace CREInterpreter.Tokens;

public class GreaterThanOrEqualToSymbolToken(int lineNumber) : IToken
{
    public string Text => ">=";

    public int LineNumber => lineNumber;

    public InterpreterException? Compile(Memory memory)
    {
        return null;
    }
}