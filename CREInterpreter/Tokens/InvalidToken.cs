namespace CREInterpreter.Tokens;

public class InvalidToken(string text, int lineNumber, InterpreterException exception) : IToken
{
    public string Text => text;

    public int LineNumber => lineNumber;

    public InterpreterException Exception => exception;

    public InterpreterException? Compile(Memory memory)
    {
        return exception;
    }
}