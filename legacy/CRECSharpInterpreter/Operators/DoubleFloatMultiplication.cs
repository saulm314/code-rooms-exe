﻿namespace CRECSharpInterpreter.Operators
{
    public class DoubleFloatMultiplication : ISpecificOperator
    {
        public Operand? LeftOperand { get; } = new(VarType.@double);
        public Operand? RightOperand { get; } = new(VarType.@double);
        public VarType? ReturnType { get; } = VarType.@double;

        public object? Calculate(object? leftValue, object? rightValue)
        {
            return (double)leftValue! * (double)rightValue!;
        }
    }
}
