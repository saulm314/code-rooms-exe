﻿namespace CREInterpreter.Tokens;

public class OpenSquareBraceSymbolToken(int lineNumber, int index) : IToken, ISymbol
{
    public string Text => "[";

    public int LineNumber => lineNumber;

    public int Index => index;
}