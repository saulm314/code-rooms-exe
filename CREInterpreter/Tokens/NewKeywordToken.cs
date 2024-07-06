namespace CREInterpreter.Tokens;

public class NewKeywordToken(int lineNumber) : IToken, IKeyword
{
    public string Text => "new";

    public int LineNumber => lineNumber;
}