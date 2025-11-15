using System;

namespace CREInterpreter.Tokens;

public class CharacterLiteralToken(ReadOnlyMemory<char> text, char value, int lineNumber, int index) : IToken, IValueTypeLiteralToken
{
    public ReadOnlyMemory<char> Text => text;

    public int LineNumber => lineNumber;

    public int Index => index;

    public char Value => value;

    public VarType _VarType => VarType.@char;

    object IValueTypeLiteralToken.Value => value;
}