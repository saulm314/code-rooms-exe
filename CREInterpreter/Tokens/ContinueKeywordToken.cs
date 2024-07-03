namespace CREInterpreter.Tokens;

public class ContinueKeywordToken(int lineNumber) : IToken
{
    public string Text => "continue";

    public int LineNumber => lineNumber;

    public InterpreterException? Compile(Memory memory)
    {
        throw new System.NotImplementedException();
    }
}