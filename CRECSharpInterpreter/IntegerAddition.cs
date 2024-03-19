namespace CRECSharpInterpreter
{
    public class IntegerAddition : IOperator
    {
        public VarType LeftType { get; } = VarType.@int;
        public VarType RightType { get; } = VarType.@int;
        public VarType ReturnType { get; } = VarType.@int;

        public object Calculate(object leftValue, object rightValue)
        {
            return (int)leftValue + (int)rightValue;
        }
    }
}
