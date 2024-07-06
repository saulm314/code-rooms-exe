namespace CREInterpreter.Tokens;

public class DivideSymbolToken(int lineNumber) : IToken, ISymbol
{
    public string Text => "/";

    public int LineNumber => lineNumber;
}