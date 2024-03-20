namespace CRECSharpInterpreter
{
    public class IntegerLessThan : IOperator
    {
        public VarType LeftType { get; } = VarType.@int;
        public VarType RightType { get; } = VarType.@int;
        public VarType ReturnType { get; } = VarType.@bool;

        public object Calculate(object leftValue, object rightValue)
        {
            return (int)leftValue < (int)rightValue;
        }
    }
}
