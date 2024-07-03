namespace CREInterpreter.Tokens;

public class InvalidToken(string text) : IToken
{
    public string Text => text;
}