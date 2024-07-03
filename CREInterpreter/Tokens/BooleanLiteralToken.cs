namespace CREInterpreter.Tokens;

public class BooleanLiteralToken(string text) : IToken
{
    public string Text => text;
}