namespace CREInterpreter.Tokens;

public class MinusSymbolToken(int lineNumber) : IToken, ISymbol
{
    public string Text => "-";

    public int LineNumber => lineNumber;
}