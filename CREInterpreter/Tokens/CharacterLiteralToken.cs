namespace CREInterpreter.Tokens;

public class CharacterLiteralToken(string text) : IToken
{
    public string Text => text;
}