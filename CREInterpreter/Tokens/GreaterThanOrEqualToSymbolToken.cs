﻿namespace CREInterpreter.Tokens;

public class GreaterThanOrEqualToSymbolToken(int lineNumber) : IToken, ISymbol
{
    public string Text => ">=";

    public int LineNumber => lineNumber;
}