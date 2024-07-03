namespace CREInterpreter.Tokens;

public class StringLiteralToken(string text) : IToken
{
    public string Text => text;
}