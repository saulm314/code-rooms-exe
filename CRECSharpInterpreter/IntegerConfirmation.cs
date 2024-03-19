namespace CRECSharpInterpreter
{
    public class IntegerConfirmation : IOperator
    {
        public VarType LeftType { get; } = null;
        public VarType RightType { get; } = VarType.@int;
        public VarType ReturnType { get; } = VarType.@int;

        public object Calculate(object leftValue, object rightValue)
        {
            return rightValue;
        }
    }
}
