namespace CREInterpreter.Tokens;

public class InvalidToken(string text, int lineNumber, int index, InterpreterException exception) : IToken
{
    public string Text => text;

    public int LineNumber => lineNumber;

    public int Index => index;

    public InterpreterException Exception => exception;
}