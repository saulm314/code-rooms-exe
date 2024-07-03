namespace CREInterpreter.Tokens;

public class SingleLineCommentToken(string text) : IToken
{
    public string Text => text;
}