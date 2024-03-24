namespace CRECSharpInterpreter.Operators
{
    public class BooleanNegation : ISpecificOperator
    {
        public Operand? LeftOperand { get; } = null;
        public Operand? RightOperand { get; } = new(VarType.@bool);
        public VarType ReturnType { get; } = VarType.@bool;

        public object Calculate(object leftValue, object rightValue)
        {
            return !(bool)rightValue;
        }
    }
}
