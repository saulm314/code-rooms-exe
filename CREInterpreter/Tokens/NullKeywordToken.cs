namespace CREInterpreter.Tokens;

public class NullKeywordToken(int lineNumber) : IToken, IKeyword
{
    public string Text => "null";

    public int LineNumber => lineNumber;
}