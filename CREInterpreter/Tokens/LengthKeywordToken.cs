namespace CREInterpreter.Tokens;

public class LengthKeywordToken(int lineNumber) : IToken
{
    public string Text => "Length";

    public int LineNumber => lineNumber;

    public InterpreterException? Compile(Memory memory)
    {
        throw new System.NotImplementedException();
    }
}