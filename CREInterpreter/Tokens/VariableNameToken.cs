namespace CREInterpreter.Tokens;

public class VariableNameToken(string text, int lineNumber) : IToken
{
    public string Text => text;

    public int LineNumber => lineNumber;
}