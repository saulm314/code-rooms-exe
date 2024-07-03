namespace CREInterpreter.Tokens;

public class IfKeywordToken(int lineNumber) : IToken
{
    public string Text => "if";

    public int LineNumber => lineNumber;

    public InterpreterException? Compile(Memory memory)
    {
        throw new System.NotImplementedException();
    }
}