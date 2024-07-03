namespace CREInterpreter.Tokens;

public class BreakKeywordToken(int lineNumber) : IToken
{
    public string Text => "break";

    public int LineNumber => lineNumber;

    public InterpreterException? Compile(Memory memory)
    {
        throw new System.NotImplementedException();
    }
}