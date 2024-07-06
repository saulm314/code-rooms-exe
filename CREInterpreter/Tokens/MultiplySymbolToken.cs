namespace CREInterpreter.Tokens;

public class MultiplySymbolToken(int lineNumber) : IToken, ISymbol
{
    public string Text => "*";

    public int LineNumber => lineNumber;
}