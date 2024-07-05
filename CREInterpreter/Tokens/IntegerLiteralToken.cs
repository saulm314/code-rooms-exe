namespace CREInterpreter.Tokens;

public class IntegerLiteralToken(string text, int value, int lineNumber) : IToken, IValueTypeLiteral
{
    public string Text => text;

    public int LineNumber => lineNumber;

    public int Value => value;

    public VarType _VarType => VarType.@int;

    object IValueTypeLiteral.Value => Value;

    public InterpreterException? Compile(Memory memory)
    {
        throw new System.NotImplementedException();
    }
}