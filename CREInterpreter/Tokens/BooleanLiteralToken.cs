namespace CREInterpreter.Tokens;

public class BooleanLiteralToken(bool value, int lineNumber) : IToken, IValueTypeLiteral
{
    public string Text => Value ? "true" : "false";

    public int LineNumber => lineNumber;

    public bool Value => value;

    public VarType _VarType => VarType.@bool;

    object IValueTypeLiteral.Value => Value;

    public InterpreterException? Compile(Memory memory)
    {
        return null;
    }
}