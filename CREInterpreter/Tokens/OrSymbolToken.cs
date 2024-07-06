﻿namespace CREInterpreter.Tokens;

public class OrSymbolToken(int lineNumber) : IToken, ISymbol
{
    public string Text => "|";

    public int LineNumber => lineNumber;
}