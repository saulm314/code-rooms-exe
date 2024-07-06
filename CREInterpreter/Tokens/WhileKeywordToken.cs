namespace CREInterpreter.Tokens;

public class WhileKeywordToken(int lineNumber) : IToken, IKeyword
{
    public string Text => "while";

    public int LineNumber => lineNumber;

    public InterpreterException? Compile(Memory memory)
    {
        return null;
    }
}