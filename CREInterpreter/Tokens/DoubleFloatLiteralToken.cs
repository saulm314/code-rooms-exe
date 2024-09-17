using System;

namespace CREInterpreter.Tokens;

public class DoubleFloatLiteralToken(ReadOnlyMemory<char> text, double value, int lineNumber, int index) : IToken, IValueTypeLiteralToken
{
    public ReadOnlyMemory<char> Text => text;

    public int LineNumber => lineNumber;

    public int Index => index;

    public double Value => value;

    public VarType _VarType => VarType.@double;

    object IValueTypeLiteralToken.Value => value;
}