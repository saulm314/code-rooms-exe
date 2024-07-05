namespace CREInterpreter.Tokens;

public class MultiplySymbolToken(int lineNumber) : IToken
{
    public string Text => "*";

    public int LineNumber => lineNumber;

    public InterpreterException? Compile(Memory memory)
    {
        return null;
    }
}