﻿namespace CRECSharpInterpreter.Operators
{
    public class DoubleFloatLessThanOrEqualTo : ISpecificOperator
    {
        public VarType LeftType { get; } = VarType.@double;
        public VarType RightType { get; } = VarType.@double;
        public VarType ReturnType { get; } = VarType.@bool;

        public object Calculate(object leftValue, object rightValue)
        {
            return (double)leftValue <= (double)rightValue;
        }
    }
}
