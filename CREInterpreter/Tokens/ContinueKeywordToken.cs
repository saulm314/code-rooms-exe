namespace CREInterpreter.Tokens;

public class ContinueKeywordToken(int lineNumber) : IToken, IKeyword
{
    public string Text => "continue";

    public int LineNumber => lineNumber;
}