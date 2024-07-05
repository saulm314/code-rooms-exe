namespace CREInterpreter.Tokens;

public class IntegerLiteralToken(int value, int lineNumber) : IToken, IValueTypeLiteral
{
    public string Text => Value.ToString();

    public int LineNumber => lineNumber;

    public int Value => value;

    public VarType _VarType => VarType.@int;

    object IValueTypeLiteral.Value => Value;

    public InterpreterException? Compile(Memory memory)
    {
        throw new System.NotImplementedException();
    }
}