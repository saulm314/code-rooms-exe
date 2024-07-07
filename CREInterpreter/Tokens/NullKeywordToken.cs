namespace CREInterpreter.Tokens;

public class NullKeywordToken(int lineNumber, int index) : IToken, IKeyword
{
    public string Text => "null";

    public int LineNumber => lineNumber;

    public int Index => index;
}