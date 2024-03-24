using System.Collections.Generic;

namespace CRECSharpInterpreter.Operators
{
    public class Equality : ISpecificOperator
    {
        private Equality(Operand leftOperand, Operand rightOperand)
        {
            LeftOperand = leftOperand;
            RightOperand = rightOperand;
        }

        public Operand? LeftOperand { get; init; }
        public Operand? RightOperand { get; init; }

        public VarType? ReturnType { get; } = VarType.@bool;

        public object? Calculate(object? leftValue, object? rightValue)
        {
            return Equals(leftValue, rightValue);
        }

        public static ISpecificOperator[] GetSpecificOperators()
        {
            List<ISpecificOperator> specificOperators = new();
            foreach (VarType varType in VarType.VarTypes)
            {
                specificOperators.Add(new Equality(new(varType), new(varType)));
                specificOperators.Add(new Equality(new(varType), new(null)));
                specificOperators.Add(new Equality(new(null), new(varType)));
            }
            specificOperators.Add(new Equality(new(null), new(null)));
            return specificOperators.ToArray();
        }
    }
}
