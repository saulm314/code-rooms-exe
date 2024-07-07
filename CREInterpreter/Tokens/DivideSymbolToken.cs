namespace CREInterpreter.Tokens;

public class DivideSymbolToken(int lineNumber, int index) : IToken, ISymbol
{
    public string Text => "/";

    public int LineNumber => lineNumber;

    public int Index => index;
}