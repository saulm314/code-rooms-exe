namespace CREInterpreter.Statements;

public interface IStatement
{
    string Text { get; }

    string? ToString() => Text;
}