namespace CRECSharpInterpreter.Operators
{
    public class BooleanNegation : ISpecificOperator
    {
        public VarType LeftType { get; } = null;
        public VarType RightType { get; } = VarType.@bool;
        public VarType ReturnType { get; } = VarType.@bool;

        public object Calculate(object leftValue, object rightValue)
        {
            return !(bool)rightValue;
        }
    }
}
