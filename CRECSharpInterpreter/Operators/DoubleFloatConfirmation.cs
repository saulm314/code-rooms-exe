namespace CRECSharpInterpreter.Operators
{
    public class DoubleFloatConfirmation : ISpecificOperator
    {
        public Operand? LeftOperand { get; } = null;
        public Operand? RightOperand { get; } = new(VarType.@double);
        public VarType ReturnType { get; } = VarType.@double;

        public object Calculate(object leftValue, object rightValue)
        {
            return rightValue;
        }
    }
}
