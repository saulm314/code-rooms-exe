using System;

namespace CREInterpreter.Tokens;

public class NewKeywordToken(ReadOnlyMemory<char> text, int lineNumber, int index) : IToken, IKeyword
{
    public ReadOnlyMemory<char> Text => text;

    public int LineNumber => lineNumber;

    public int Index => index;

    string IKeyword.Text { get; } = text.ToString();
}