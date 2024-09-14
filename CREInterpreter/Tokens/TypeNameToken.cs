using System;

namespace CREInterpreter.Tokens;

public class TypeNameToken(ReadOnlyMemory<char> text, VarType varType, int lineNumber, int index) : IToken
{
    public ReadOnlyMemory<char> Text => text;

    public int LineNumber => lineNumber;

    public int Index => index;

    public VarType _VarType => varType;
}