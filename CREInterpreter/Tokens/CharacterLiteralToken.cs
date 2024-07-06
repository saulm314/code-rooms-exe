namespace CREInterpreter.Tokens;

public class CharacterLiteralToken(string text, char value, int lineNumber) : IToken, IValueTypeLiteral
{
    public string Text => text;

    public int LineNumber => lineNumber;

    public char Value => value;

    public VarType _VarType => VarType.@char;

    object IValueTypeLiteral.Value => Value;
}