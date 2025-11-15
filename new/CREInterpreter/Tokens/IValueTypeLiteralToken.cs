namespace CREInterpreter.Tokens;

public interface IValueTypeLiteralToken : IToken
{
    VarType _VarType { get; }
    object Value { get; }
}