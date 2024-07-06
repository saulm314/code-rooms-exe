namespace CREInterpreter.Tokens;

public class IfKeywordToken(int lineNumber, int index) : IToken, IKeyword
{
    public string Text => "if";

    public int LineNumber => lineNumber;

    public int Index => index;
}