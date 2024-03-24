namespace CRECSharpInterpreter.Operators
{
    public class IntegerConfirmation : ISpecificOperator
    {
        public Operand? LeftOperand { get; } = null;
        public Operand? RightOperand { get; } = new(VarType.@int);
        public VarType ReturnType { get; } = VarType.@int;

        public object Calculate(object leftValue, object rightValue)
        {
            return rightValue;
        }
    }
}
