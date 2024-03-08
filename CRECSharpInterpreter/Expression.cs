namespace CRECSharpInterpreter
{
    public class Expression
    {
        public Expression(KeyString[] keyStrings)
        {
            KeyStrings = keyStrings;
            _VarType = ComputeVarType();
        }

        public KeyString[] KeyStrings { get; init; }

        public VarType _VarType { get; init; }
        public object Value { get; private set; }

        private VarType ComputeVarType()
        {
            if (KeyStrings.Length != 1)
                throw new ExpressionException(this, $"Unrecognised expression of length {KeyStrings.Length}");
            if (KeyStrings[0]._Type == KeyString.Type.Variable)
            {
                Variable variable = Info.Instance.DeclaredVariables.Find(var => var.Name == KeyStrings[0].Text);
                return variable._VarType;
            }
            if (KeyStrings[0]._Literal != null)
            {
                Literal literal = KeyStrings[0]._Literal;
                return literal._VarType;
            }
            throw new ExpressionException(this, $"Could not parse key string {KeyStrings[0]} in expression");
        }

        public void Compute()
        {
            KeyString keyString = KeyStrings[0];
            if (keyString._Type == KeyString.Type.Variable)
            {
                Variable variable = Info.Instance.DeclaredVariables.Find(var => var.Name == keyString.Text);
                Value = variable.Value;
                return;
            }
            Value = keyString._Literal.Value;
        }

        public class ExpressionException : InterpreterException
        {
            public ExpressionException(Expression expression, string message = null) : base(message)
            {
                this.expression = expression;
            }

            public Expression expression;
        }
    }
}
