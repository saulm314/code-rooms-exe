namespace CREInterpreter;

public interface ISymbol
{
    string Text { get; }

    string? ToString() => Text;
}