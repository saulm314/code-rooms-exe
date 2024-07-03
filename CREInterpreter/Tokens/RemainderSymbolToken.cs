namespace CREInterpreter.Tokens;

public class RemainderSymbolToken(int lineNumber) : IToken
{
    public string Text => "%";

    public int LineNumber => lineNumber;

    public InterpreterException? Compile(Memory memory)
    {
        throw new System.NotImplementedException();
    }
}