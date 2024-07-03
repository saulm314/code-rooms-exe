namespace CREInterpreter.Tokens;

public class MultiLineCommentToken(string text) : IToken
{
    public string Text => text;
}