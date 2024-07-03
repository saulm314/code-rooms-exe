namespace CREInterpreter.Tokens;

public class ForKeywordToken(int lineNumber) : IToken
{
    public string Text => "for";

    public int LineNumber => lineNumber;

    public InterpreterException? Compile(Memory memory)
    {
        throw new System.NotImplementedException();
    }
}