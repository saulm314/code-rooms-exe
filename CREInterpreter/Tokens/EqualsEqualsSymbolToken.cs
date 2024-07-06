namespace CREInterpreter.Tokens;

public class EqualsEqualsSymbolToken(int lineNumber) : IToken, ISymbol
{
    public string Text => "==";

    public int LineNumber => lineNumber;
}