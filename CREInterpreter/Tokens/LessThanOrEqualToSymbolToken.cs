namespace CREInterpreter.Tokens;

public class LessThanOrEqualToSymbolToken(int lineNumber) : IToken, ISymbol
{
    public string Text => "<=";

    public int LineNumber => lineNumber;
}