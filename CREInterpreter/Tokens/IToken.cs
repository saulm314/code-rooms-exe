namespace CREInterpreter.Tokens;

public interface IToken
{
    string Text { get; }

    string? ToString() => Text;
}