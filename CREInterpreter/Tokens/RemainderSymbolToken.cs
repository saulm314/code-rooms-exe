﻿namespace CREInterpreter.Tokens;

public class RemainderSymbolToken(int lineNumber) : IToken, ISymbol
{
    public string Text => "%";

    public int LineNumber => lineNumber;
}