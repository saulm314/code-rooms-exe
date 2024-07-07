namespace CREInterpreter.Tokens;

public class StringLiteralToken(string text, string value, int lineNumber, int index) : IToken
{
    public string Text => text;

    public int LineNumber => lineNumber;

    public int Index => index;

    public string Value => value;
}