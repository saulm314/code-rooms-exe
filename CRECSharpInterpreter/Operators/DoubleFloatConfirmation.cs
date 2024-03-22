namespace CRECSharpInterpreter.Operators
{
    public class DoubleFloatConfirmation : ISpecificOperator
    {
        public VarType LeftType { get; } = null;
        public VarType RightType { get; } = VarType.@double;
        public VarType ReturnType { get; } = VarType.@double;

        public object Calculate(object leftValue, object rightValue)
        {
            return rightValue;
        }
    }
}
