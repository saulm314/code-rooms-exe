namespace CREInterpreter.Tokens;

public class InvalidToken(string text, int lineNumber, InterpreterException Exception) : IToken
{
    public string Text => text;

    public int LineNumber => lineNumber;

    public InterpreterException? Compile(Memory memory)
    {
        return Exception;
    }
}