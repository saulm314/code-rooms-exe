namespace CREInterpreter.Tokens;

public class IntegerLiteralToken(string text, int value, int lineNumber, int index) : IToken, IValueTypeLiteral
{
    public string Text => text;

    public int LineNumber => lineNumber;

    public int Index => index;

    public int Value => value;

    public VarType _VarType => VarType.@int;

    object IValueTypeLiteral.Value => Value;
}