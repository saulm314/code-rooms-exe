using System.Collections.Generic;

namespace CRECSharpInterpreter
{
    public class Operator
    {
        public string Symbol { get; init; }
        public OperatorPriority Priority { get; init; }
        public IOperator[] PotentialSpecificOperators { get; init; }

        private Operator(string symbol, OperatorPriority priority, IOperator[] potentialSpecificOperators)
        {
            Symbol = symbol;
            Priority = priority;
            PotentialSpecificOperators = potentialSpecificOperators;

            Operators.Add(this);
        }

        public IOperator GetSpecificOperator(VarType leftType, VarType rightType)
        {
            foreach (IOperator specificOperator in PotentialSpecificOperators)
                if (specificOperator.LeftType == leftType && specificOperator.RightType == rightType)
                    return specificOperator;
            return null;
        }

        public static List<Operator> Operators { get; } = new();

        public static Operator Plus { get; } = new("+", OperatorPriority.LeftToRight, new IOperator[]
            {
                new IntegerAddition(),
                new IntegerConfirmation()
            });

        public static Operator GetOperator(string symbol)
        {
            foreach (Operator @operator in Operators)
                if (@operator.Symbol == symbol)
                    return @operator;
            return null;
        }



        public class OperatorException : InterpreterException
        {
            public OperatorException(Operator @operator, string message = null) : base(message)
            {
                this.@operator = @operator;
            }

            public Operator @operator;
        }
    }
}
