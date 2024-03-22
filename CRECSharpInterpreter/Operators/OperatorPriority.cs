namespace CRECSharpInterpreter.Operators
{
    public enum OperatorPriority
    {
        LeftToRight,    // e.g. 5 + 3 - 7
        ImmediateExpressions,   // e.g. 5 + 3 * 5    (for the * operator, evaluate expressions immediately to its left/right first)
        AllExpressions  // e.g. 5 + 3 == 5 + 3       (for the == operator, evaluate all expressions to its left/right first)
    }
}
