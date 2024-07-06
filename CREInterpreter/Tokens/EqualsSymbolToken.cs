﻿namespace CREInterpreter.Tokens;

public class EqualsSymbolToken(int lineNumber) : IToken, ISymbol
{
    public string Text => "=";

    public int LineNumber => lineNumber;
}