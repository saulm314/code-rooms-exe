namespace CRECSharpInterpreter.Operators
{
    public class DoubleFloatAddition : ISpecificOperator
    {
        public VarType LeftType { get; } = VarType.@double;
        public VarType RightType { get; } = VarType.@double;
        public VarType ReturnType { get; } = VarType.@double;

        public object Calculate(object leftValue, object rightValue)
        {
            return (double)leftValue + (double)rightValue;
        }
    }
}
