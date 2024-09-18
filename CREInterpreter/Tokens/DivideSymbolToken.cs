﻿using System;

namespace CREInterpreter.Tokens;

public class DivideSymbolToken(ReadOnlyMemory<char> text, int lineNumber, int index) : IToken, ISymbolToken
{
    public ReadOnlyMemory<char> Text => text;

    public int LineNumber => lineNumber;

    public int Index => index;

    string ISymbolToken.Text { get; } = text.ToString();
}