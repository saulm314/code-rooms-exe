namespace CREInterpreter.Tokens;

public class VariableNameToken(string text) : IToken
{
    public string Text => text;
}