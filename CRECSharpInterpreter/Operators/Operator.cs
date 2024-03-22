using System.Collections.Generic;

namespace CRECSharpInterpreter.Operators
{
    public class Operator : IExpressionComponent
    {
        public string Symbol { get; init; }
        public OperatorPriority Priority { get; init; }
        public ISpecificOperator[] PotentialSpecificOperators { get; init; }

        public KeyString KeyString { get => new(Symbol); }

        public ExpressionComponentType _Type { get; } = ExpressionComponentType.Operator;

        private Operator(string symbol, OperatorPriority priority, ISpecificOperator[] potentialSpecificOperators)
        {
            Symbol = symbol;
            Priority = priority;
            PotentialSpecificOperators = potentialSpecificOperators;

            Operators.Add(this);
        }

        public ISpecificOperator GetSpecificOperator(VarType leftType, VarType rightType)
        {
            foreach (ISpecificOperator specificOperator in PotentialSpecificOperators)
                if (specificOperator.LeftType == leftType && specificOperator.RightType == rightType)
                    return specificOperator;
            return null;
        }

        public static List<Operator> Operators { get; } = new();

        public static Operator Plus { get; } = new("+", OperatorPriority.LeftToRight, new ISpecificOperator[]
            {
                new IntegerAddition(),
                new IntegerConfirmation(),
                new DoubleFloatAddition(),
                new DoubleFloatConfirmation()
            });

        public static Operator Minus { get; } = new("-", OperatorPriority.LeftToRight, new ISpecificOperator[]
            {
                new IntegerSubtraction(),
                new IntegerNegation(),
                new DoubleFloatSubtraction(),
                new DoubleFloatConfirmation()
            });

        public static Operator Multiply { get; } = new("*", OperatorPriority.ImmediateExpressions, new ISpecificOperator[]
            {
                new IntegerMultiplication(),
                new DoubleFloatMultiplication()
            });

        public static Operator Divide { get; } = new("/", OperatorPriority.ImmediateExpressions, new ISpecificOperator[]
            {
                new IntegerDivision(),
                new DoubleFloatDivision()
            });

        public static Operator LessThan { get; } = new("<", OperatorPriority.AllExpressions, new ISpecificOperator[]
            {
                new IntegerLessThan(),
                new DoubleFloatLessThan()
            });

        public static Operator GreaterThan { get; } = new(">", OperatorPriority.AllExpressions, new ISpecificOperator[]
            {
                new IntegerGreaterThan(),
                new DoubleFloatGreaterThan()
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
