namespace CREInterpreter.Tokens;

public interface IToken
{
    string Text { get; }

    int LineNumber { get; }

    string? ToString() => Text;
}