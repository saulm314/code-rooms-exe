namespace CRECSharpInterpreter.Operators
{
    public class BooleanConditionalAnd : ISpecificOperator
    {

        public Operand? LeftOperand { get; } = new(VarType.@bool);
        public Operand? RightOperand { get; } = new(VarType.@bool);
        public VarType ReturnType { get; } = VarType.@bool;

        public object Calculate(object leftValue, object rightValue)
        {
            return (bool)leftValue && (bool)rightValue;
        }
    }
}
