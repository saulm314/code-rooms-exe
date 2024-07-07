namespace CREInterpreter.Tokens;

public class ForKeywordToken(int lineNumber, int index) : IToken, IKeyword
{
    public string Text => "for";

    public int LineNumber => lineNumber;

    public int Index => index;
}