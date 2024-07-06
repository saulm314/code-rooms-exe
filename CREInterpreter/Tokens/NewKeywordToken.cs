namespace CREInterpreter.Tokens;

public class NewKeywordToken(int lineNumber, int index) : IToken, IKeyword
{
    public string Text => "new";

    public int LineNumber => lineNumber;

    public int Index => index;
}