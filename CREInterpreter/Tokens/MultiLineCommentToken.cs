namespace CREInterpreter.Tokens;

public class MultiLineCommentToken(string text, int lineNumber, int index) : IToken
{
    public string Text => text;

    public int LineNumber => lineNumber;

    public int Index => index;
}