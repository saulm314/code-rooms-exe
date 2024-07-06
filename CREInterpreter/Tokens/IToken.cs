namespace CREInterpreter.Tokens;

public interface IToken
{
    string Text { get; }

    int LineNumber { get; }

    // index of first character where token starts in the chunk's text
    int Index { get; }

    string? ToString() => Text;
}