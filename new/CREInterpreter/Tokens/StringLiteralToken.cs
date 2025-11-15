using System;

namespace CREInterpreter.Tokens;

public class StringLiteralToken(ReadOnlyMemory<char> text, ReadOnlyMemory<char> value, int lineNumber, int index) : IToken
{
    public ReadOnlyMemory<char> Text => text;

    public int LineNumber => lineNumber;

    public int Index => index;

    public ReadOnlyMemory<char> Value => value;
}