﻿namespace CREInterpreter.Tokens;

public class XorSymbolToken(int lineNumber) : IToken, ISymbol
{
    public string Text => "^";

    public int LineNumber => lineNumber;
}