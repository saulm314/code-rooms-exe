namespace CREInterpreter.Tokens;

public class LessThanSymbolToken(int lineNumber) : IToken, ISymbol
{
    public string Text => "<";

    public int LineNumber => lineNumber;
}