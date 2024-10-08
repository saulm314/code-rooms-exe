﻿using System;

namespace CREInterpreter.Tokens;

public class IntegerLiteralToken(ReadOnlyMemory<char> text, int value, int lineNumber, int index) : IToken, IValueTypeLiteralToken
{
    public ReadOnlyMemory<char> Text => text;

    public int LineNumber => lineNumber;

    public int Index => index;

    public int Value => value;

    public VarType _VarType => VarType.@int;

    object IValueTypeLiteralToken.Value => value;
}