namespace CREInterpreter.Tokens;

public class WhileKeywordToken(int lineNumber, int index) : IToken, IKeyword
{
    public string Text => "while";

    public int LineNumber => lineNumber;

    public int Index => index;
}