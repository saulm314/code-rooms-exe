using System.Collections.Generic;
using System.Linq;

namespace CRECSharpInterpreter.Operators
{
    public class Operator : IExpressionComponent
    {
        public string Symbol { get; init; }
        public OperatorPriority Priority { get; init; }
        public ISpecificOperator[] PotentialSpecificOperators { get; init; }

        public KeyString KeyString { get => new(Symbol); }

        public KeyString[] GetKeyStrings() => new KeyString[] { KeyString };

        public ExpressionComponentType _Type { get; } = ExpressionComponentType.Operator;

        private Operator(string symbol, OperatorPriority priority, ISpecificOperator[] potentialSpecificOperators)
        {
            Symbol = symbol;
            Priority = priority;
            PotentialSpecificOperators = potentialSpecificOperators;

            Operators.Add(this);
        }

        public ISpecificOperator? GetSpecificOperator(Operand? leftOperand, Operand? rightOperand)
        {
            foreach (ISpecificOperator specificOperator in PotentialSpecificOperators)
                if (Equals(specificOperator.LeftOperand, leftOperand) && Equals(specificOperator.RightOperand, rightOperand))
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
                new DoubleFloatNegation()
            });

        public static Operator Multiply { get; } = new("*", OperatorPriority.ImmediateUnits, new ISpecificOperator[]
            {
                new IntegerMultiplication(),
                new DoubleFloatMultiplication()
            });

        public static Operator Divide { get; } = new("/", OperatorPriority.ImmediateUnits, new ISpecificOperator[]
            {
                new IntegerDivision(),
                new DoubleFloatDivision()
            });

        public static Operator Remainder { get; } = new("%", OperatorPriority.ImmediateUnits, new ISpecificOperator[]
            {
                new IntegerRemainder()
            });

        public static Operator LessThan { get; } = new("<", OperatorPriority.AllUnits, new ISpecificOperator[]
            {
                new IntegerLessThan(),
                new DoubleFloatLessThan()
            });

        public static Operator LessThanOrEqualTo { get; } = new("<=", OperatorPriority.AllUnits, new ISpecificOperator[]
            {
                new IntegerLessThanOrEqualTo(),
                new DoubleFloatLessThanOrEqualTo()
            });

        public static Operator GreaterThan { get; } = new(">", OperatorPriority.AllUnits, new ISpecificOperator[]
            {
                new IntegerGreaterThan(),
                new DoubleFloatGreaterThan()
            });

        public static Operator GreaterThanOrEqualTo { get; } = new(">=", OperatorPriority.AllUnits, new ISpecificOperator[]
            {
                new IntegerGreaterThanOrEqualTo(),
                new DoubleFloatGreaterThanOrEqualTo()
            });

        public static Operator Not { get; } = new("!", OperatorPriority.LeftToRight, new ISpecificOperator[]
            {
                new BooleanNegation()
            });

        public static Operator And { get; } = new("&", OperatorPriority.LeftToRight, new ISpecificOperator[]
            {
                new BooleanAnd()
            });

        public static Operator ConditionalAnd { get; } = new("&&", OperatorPriority.LeftToRight, new ISpecificOperator[]
            {
                new BooleanConditionalAnd()
            });

        public static Operator Or { get; } = new("|", OperatorPriority.LeftToRight, new ISpecificOperator[]
            {
                new BooleanOr()
            });

        public static Operator ConditionalOr { get; } = new("||", OperatorPriority.LeftToRight, new ISpecificOperator[]
            {
                new BooleanConditionalOr()
            });

        public static Operator Xor { get; } = new("^", OperatorPriority.LeftToRight, new ISpecificOperator[]
            {
                new BooleanXor()
            });

        public static new Operator Equals { get; } = new("==", OperatorPriority.AllUnits, Equality.GetSpecificOperators());

        public static Operator NotEqual { get; } = new("!=", OperatorPriority.AllUnits, Inequality.GetSpecificOperators());

        public static Operator[] Casts { get; } = GetCastOperators();
        private static Operator[] GetCastOperators()
        {
            Operator[] operators = new Operator[VarType.VarTypes.Count];
            for (int i = 0; i < operators.Length; i++)
            {
                VarType varType = VarType.VarTypes[i];
                ISpecificOperator[] specificOperators = Cast.GetCastsForReturnType(varType);
                operators[i] = new($"({varType.Name})", OperatorPriority.LeftToRight, specificOperators);
            }
            return operators;
        }

        public bool IsCast { get => Casts.Contains(this); }

        public static Operator? GetOperator(string symbol)
        {
            foreach (Operator @operator in Operators)
                if (@operator.Symbol == symbol)
                    return @operator;
            return null;
        }

        public override string ToString()
        {
            return Symbol;
        }

        public class OperatorException : InterpreterException
        {
            public OperatorException(Operator? @operator, string? message = null) : base(message)
            {
                this.@operator = @operator;
            }

            public Operator? @operator;
        }
    }
}
