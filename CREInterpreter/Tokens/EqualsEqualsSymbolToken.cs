using System;

namespace CREInterpreter.Tokens;

public class EqualsEqualsSymbolToken(ReadOnlyMemory<char> text, int lineNumber, int index) : IToken, ISymbol
{
    public ReadOnlyMemory<char> Text => text;

    public int LineNumber => lineNumber;

    public int Index => index;

    string ISymbol.Text { get; } = text.ToString();
}