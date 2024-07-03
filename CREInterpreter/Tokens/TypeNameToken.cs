namespace CREInterpreter.Tokens;

public class TypeNameToken(string text) : IToken
{
    public string Text => text;
}