namespace CREInterpreter.Tokens;

public class NotEqualsSymbolToken(int lineNumber) : IToken, ISymbol
{
    public string Text => "!=";

    public int LineNumber => lineNumber;
}