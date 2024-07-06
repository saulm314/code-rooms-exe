namespace CREInterpreter.Tokens;

public class IfKeywordToken(int lineNumber) : IToken, IKeyword
{
    public string Text => "if";

    public int LineNumber => lineNumber;

    public InterpreterException? Compile(Memory memory)
    {
        return null;
    }
}