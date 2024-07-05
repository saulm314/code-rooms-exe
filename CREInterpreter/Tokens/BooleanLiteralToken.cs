namespace CREInterpreter.Tokens;

public class BooleanLiteralToken(string text, bool value, int lineNumber) : IToken, IValueTypeLiteral
{
    public string Text => text;

    public int LineNumber => lineNumber;

    public bool Value => value;

    public VarType _VarType => VarType.@bool;

    object IValueTypeLiteral.Value => Value;

    public InterpreterException? Compile(Memory memory)
    {
        return null;
    }
}