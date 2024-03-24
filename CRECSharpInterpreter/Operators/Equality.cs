namespace CRECSharpInterpreter.Operators
{
    public class Equality : ISpecificOperator
    {
        public Equality(VarType inputType)
        {
            LeftType = inputType;
            RightType = inputType;
        }

        public VarType LeftType { get; init; }
        public VarType RightType { get; init; }
        public VarType ReturnType { get; } = VarType.@bool;

        public object Calculate(object leftValue, object rightValue)
        {
            return Equals(leftValue, rightValue);
        }
    }
}
