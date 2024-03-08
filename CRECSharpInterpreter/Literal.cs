using System;

namespace CRECSharpInterpreter
{
    public class Literal
    {
        public Literal(VarType varType, object value)
        {
            _VarType = varType;
            Value = value;
        }

        public VarType _VarType { get; init; }
        public object Value { get; init; }

        public class LiteralException : Exception
        {
            public LiteralException(Literal literal, string message = null) : base(message)
            {
                this.literal = literal;
            }

            public Literal literal;
        }
    }
}
