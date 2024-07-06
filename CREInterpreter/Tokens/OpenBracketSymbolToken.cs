namespace CREInterpreter.Tokens;

public class OpenBracketSymbolToken(int lineNumber) : IToken, ISymbol
{
    public string Text => "(";

    public int LineNumber => lineNumber;
}