namespace CREInterpreter.Tokens;

public class BreakKeywordToken(int lineNumber) : IToken, IKeyword
{
    public string Text => "break";

    public int LineNumber => lineNumber;

    public InterpreterException? Compile(Memory memory)
    {
        return null;
    }
}