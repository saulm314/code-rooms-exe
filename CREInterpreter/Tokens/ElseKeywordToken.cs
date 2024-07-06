namespace CREInterpreter.Tokens;

public class ElseKeywordToken(int lineNumber) : IToken, IKeyword
{
    public string Text => "else";

    public int LineNumber => lineNumber;
}