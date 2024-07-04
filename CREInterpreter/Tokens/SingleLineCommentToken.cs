namespace CREInterpreter.Tokens;

public class SingleLineCommentToken(string text, int lineNumber) : IToken
{
    public string Text => text;

    public int LineNumber => lineNumber;

    public InterpreterException? Compile(Memory memory)
    {
        return null;
    }
}