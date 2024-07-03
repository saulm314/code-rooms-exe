namespace CREInterpreter.Tokens;

public class ElseKeywordToken(int lineNumber) : IToken
{
    public string Text => "else";

    public int LineNumber => lineNumber;

    public InterpreterException? Compile(Memory memory)
    {
        throw new System.NotImplementedException();
    }
}