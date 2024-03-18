using System.Collections.Generic;

namespace CRECSharpInterpreter
{
    public class Operator
    {
        public string Symbol { get; init; }

        private Operator(string symbol)
        {
            Symbol = symbol;

            Operators.Add(this);
        }

        public static List<Operator> Operators { get; } = new();

        public static Operator Plus { get; } = new("+");

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
