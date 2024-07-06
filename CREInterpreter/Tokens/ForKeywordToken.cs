namespace CREInterpreter.Tokens;

public class ForKeywordToken(int lineNumber) : IToken, IKeyword
{
    public string Text => "for";

    public int LineNumber => lineNumber;

    public InterpreterException? Compile(Memory memory)
    {
        return null;
    }
}