namespace CREInterpreter.Tokens;

public class CommaSymbolToken(int lineNumber) : IToken, ISymbol
{
    public string Text => ",";

    public int LineNumber => lineNumber;
}