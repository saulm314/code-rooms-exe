namespace CREInterpreter;

public interface IValueTypeLiteral
{
    VarType _VarType { get; }
    object Value { get; }
}