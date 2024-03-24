namespace CRECSharpInterpreter.Operators
{
    public class DoubleFloatGreaterThan : ISpecificOperator
    {
        public Operand? LeftOperand { get; } = new(VarType.@double);
        public Operand? RightOperand { get; } = new(VarType.@double);
        public VarType? ReturnType { get; } = VarType.@bool;

        public object? Calculate(object? leftValue, object? rightValue)
        {
            return (double)leftValue! > (double)rightValue!;
        }
    }
}
