namespace CREInterpreter.Tokens;

public class LengthKeywordToken(int lineNumber, int index) : IToken, IKeyword
{
    public string Text => "Length";

    public int LineNumber => lineNumber;

    public int Index => index;
}