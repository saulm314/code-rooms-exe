namespace CRECSharpInterpreter.Operators
{
    public class IntegerSubtraction : ISpecificOperator
    {
        public Operand? LeftOperand { get; } = new(VarType.@int);
        public Operand? RightOperand { get; } = new(VarType.@int);
        public VarType ReturnType { get; } = VarType.@int;

        public object Calculate(object leftValue, object rightValue)
        {
            return (int)leftValue - (int)rightValue;
        }
    }
}
