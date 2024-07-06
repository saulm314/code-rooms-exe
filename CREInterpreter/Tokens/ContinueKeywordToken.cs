namespace CREInterpreter.Tokens;

public class ContinueKeywordToken(int lineNumber, int index) : IToken, IKeyword
{
    public string Text => "continue";

    public int LineNumber => lineNumber;

    public int Index => index;
}