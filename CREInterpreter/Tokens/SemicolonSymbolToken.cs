namespace CREInterpreter.Tokens;

public class SemicolonSymbolToken(int lineNumber) : IToken, ISymbol
{
    public string Text => ";";

    public int LineNumber => lineNumber;
}