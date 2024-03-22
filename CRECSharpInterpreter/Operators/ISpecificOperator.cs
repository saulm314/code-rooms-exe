namespace CRECSharpInterpreter.Operators
{
    public interface ISpecificOperator
    {
        public VarType LeftType { get; }
        public VarType RightType { get; }
        public VarType ReturnType { get; }

        // left and right values to be determined based on priority
        // e.g. if this is the * operator, and if the overall expression is 3 + 5 * 4 + 3
        //      then leftValue should be 5 and rightValue should be 4
        // e.g. if this is the == operator, and if the overall expression is 3 + 5 == 2 + 6
        //      then leftValue should be 3 + 5 and rightValue should be 2 + 6
        public object Calculate(object leftValue, object rightValue);
    }
}
