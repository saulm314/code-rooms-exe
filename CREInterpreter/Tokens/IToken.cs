namespace CREInterpreter.Tokens;

public interface IToken
{
    string Text { get; }

    int LineNumber { get; }

    InterpreterException? Compile(Memory memory);

    string? ToString() => Text;
}