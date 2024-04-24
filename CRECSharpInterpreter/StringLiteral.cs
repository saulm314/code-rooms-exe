namespace CRECSharpInterpreter
{
    public class StringLiteral
    {

        public StringLiteral(string keyString)
        {
            Value = keyString.Replace("\"", string.Empty);
            if (Value.Contains('\n'))
                throw new StringLiteralException(this, "String literal cannot contain newline character");
        }

        public string Value { get; init; }

        public class StringLiteralException : InterpreterException
        {
            public StringLiteralException(StringLiteral? stringLiteral, string? message = null) : base(message)
            {
                this.stringLiteral = stringLiteral;
            }

            public StringLiteral? stringLiteral;
        }
    }
}
