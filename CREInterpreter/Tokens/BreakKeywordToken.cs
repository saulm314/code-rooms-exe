namespace CREInterpreter.Tokens;

public class BreakKeywordToken(int lineNumber, int index) : IToken, IKeyword
{
    public string Text => "break";

    public int LineNumber => lineNumber;

    public int Index => index;
}