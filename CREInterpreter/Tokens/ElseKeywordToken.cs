namespace CREInterpreter.Tokens;

public class ElseKeywordToken(int lineNumber, int index) : IToken, IKeyword
{
    public string Text => "else";

    public int LineNumber => lineNumber;

    public int Index => index;
}