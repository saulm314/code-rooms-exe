﻿namespace CREInterpreter.Tokens;

public class DotSymbolToken(int lineNumber) : IToken, ISymbol
{
    public string Text => ".";

    public int LineNumber => lineNumber;
}