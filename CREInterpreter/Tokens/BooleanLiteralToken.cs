using System;

namespace CREInterpreter.Tokens;

public class BooleanLiteralToken(ReadOnlyMemory<char> text, bool value, int lineNumber, int index) : IToken, IValueTypeLiteralToken
{
    public ReadOnlyMemory<char> Text => text;

    public int LineNumber => lineNumber;

    public int Index => index;

    public bool Value => value;

    public VarType _VarType => VarType.@bool;

    object IValueTypeLiteralToken.Value => value;
}