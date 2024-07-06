namespace CREInterpreter;

public interface IKeyword
{
    string Text { get; }

    string? ToString() => Text;
}