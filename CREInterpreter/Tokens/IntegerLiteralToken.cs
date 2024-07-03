namespace CREInterpreter.Tokens;

public class IntegerLiteralToken(string text) : IToken
{
    public string Text => text;
}