namespace CREInterpreter.Tokens;

public class DoubleFloatLiteralToken(string text, double value, int lineNumber, int index) : IToken, IValueTypeLiteral
{
    public string Text => text;

    public int LineNumber => lineNumber;

    public int Index => index;

    public double Value => value;

    public VarType _VarType => VarType.@double;

    object IValueTypeLiteral.Value => Value;
}