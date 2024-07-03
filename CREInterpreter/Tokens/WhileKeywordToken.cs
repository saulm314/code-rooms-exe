namespace CREInterpreter.Tokens;

public class WhileKeywordToken(int lineNumber) : IToken
{
    public string Text => "while";

    public int LineNumber => lineNumber;

    public InterpreterException? Compile(Memory memory)
    {
        throw new System.NotImplementedException();
    }
}