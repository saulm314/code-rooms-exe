﻿namespace CRECSharpInterpreter.Operators
{
    public class BooleanXor : ISpecificOperator
    {
        public VarType LeftType { get; } = VarType.@bool;
        public VarType RightType { get; } = VarType.@bool;
        public VarType ReturnType { get; } = VarType.@bool;

        public object Calculate(object leftValue, object rightValue)
        {
            return (bool)leftValue ^ (bool)rightValue;
        }
    }
}
