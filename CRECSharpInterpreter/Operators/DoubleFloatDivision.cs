namespace CRECSharpInterpreter.Operators
{
    public class DoubleFloatDivision : ISpecificOperator
    {
        public Operand? LeftOperand { get; } = new(VarType.@double);
        public Operand? RightOperand { get; } = new(VarType.@double);
        public VarType? ReturnType { get; } = VarType.@double;

        public object? Calculate(object? leftValue, object? rightValue)
        {
            if ((double)rightValue! == 0.0)
                throw new System.DivideByZeroException();
            return (double)leftValue! / (double)rightValue!;
        }
    }
}
