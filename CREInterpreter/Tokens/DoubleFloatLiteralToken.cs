namespace CREInterpreter.Tokens;

public class DoubleFloatLiteralToken(string text, int lineNumber) : IToken
{
    public string Text => text;

    public int LineNumber => lineNumber;

    public InterpreterException? Compile(Memory memory)
    {
        throw new System.NotImplementedException();
    }
}