namespace CREInterpreter.Tokens;

public class NewKeywordToken(int lineNumber) : IToken
{
    public string Text => "new";

    public int LineNumber => lineNumber;

    public InterpreterException? Compile(Memory memory)
    {
        throw new System.NotImplementedException();
    }
}