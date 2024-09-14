using System;

namespace CREInterpreter.Tokens;

public interface IToken
{
    ReadOnlyMemory<char> Text { get; }

    int LineNumber { get; }

    // index of first character where token starts in the chunk's text
    int Index { get; }

    string? ToString() => Text.ToString();
}