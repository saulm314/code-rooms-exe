namespace CREInterpreter.Tokens;

public class DoubleFloatLiteralToken(string text) : IToken
{
    public string Text => text;
}