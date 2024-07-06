namespace CREInterpreter.Tokens;

public class AndSymbolToken(int lineNumber) : IToken, ISymbol
{
    public string Text => "&";

    public int LineNumber => lineNumber;
}