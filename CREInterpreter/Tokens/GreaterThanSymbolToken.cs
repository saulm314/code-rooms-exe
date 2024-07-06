namespace CREInterpreter.Tokens;

public class GreaterThanSymbolToken(int lineNumber, int index) : IToken, ISymbol
{
    public string Text => ">";

    public int LineNumber => lineNumber;

    public int Index => index;
}