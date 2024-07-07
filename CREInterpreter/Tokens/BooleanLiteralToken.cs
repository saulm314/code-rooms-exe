namespace CREInterpreter.Tokens;

public class BooleanLiteralToken(string text, bool value, int lineNumber, int index) : IToken, IValueTypeLiteral
{
    public string Text => text;

    public int LineNumber => lineNumber;

    public int Index => index;

    public bool Value => value;

    public VarType _VarType => VarType.@bool;

    object IValueTypeLiteral.Value => Value;
}