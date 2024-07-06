namespace CREInterpreter.Tokens;

public class MultiLineCommentToken(string text, int lineNumber) : IToken
{
    public string Text => text;

    public int LineNumber => lineNumber;
}