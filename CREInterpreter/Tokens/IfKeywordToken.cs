using System;

namespace CREInterpreter.Tokens;

public class IfKeywordToken(ReadOnlyMemory<char> text, int lineNumber, int index) : IToken, IKeywordToken
{
    public ReadOnlyMemory<char> Text => text;

    public int LineNumber => lineNumber;

    public int Index => index;

    string IKeywordToken.Text { get; } = text.ToString();
}