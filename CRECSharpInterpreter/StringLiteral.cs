namespace CRECSharpInterpreter
{
    public class StringLiteral
    {

        public StringLiteral(string keyString)
        {
            Value = keyString.Replace("\"", string.Empty);
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
