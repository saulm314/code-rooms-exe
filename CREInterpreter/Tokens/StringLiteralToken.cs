namespace CREInterpreter.Tokens;

public class StringLiteralToken(string text, string value, int lineNumber) : IToken
{
    public string Text => text;

    public int LineNumber => lineNumber;

    public string Value => value;

    public InterpreterException? Compile(Memory memory)
    {
        return null;
    }
}