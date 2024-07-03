namespace CREInterpreter.Tokens;

public class NullKeywordToken(int lineNumber) : IToken
{
    public string Text => "null";

    public int LineNumber => lineNumber;

    public InterpreterException? Compile(Memory memory)
    {
        throw new System.NotImplementedException();
    }
}