﻿namespace CREInterpreter.Tokens;

public class PlusSymbolToken(int lineNumber) : IToken, ISymbol
{
    public string Text => "+";

    public int LineNumber => lineNumber;
}