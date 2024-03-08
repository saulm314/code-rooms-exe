using System;

namespace CRECSharpInterpreter
{
    public class Expression
    {
        public Expression(KeyString[] keyStrings)
        {
            KeyStrings = keyStrings;
            VarType varType;
            string errorMessage;
            bool isValid = IsValid(out varType, out errorMessage);
            _VarType = varType;
            ErrorMessage = errorMessage;

            if (!isValid)
                throw new ExpressionException(this, ErrorMessage);
        }

        public KeyString[] KeyStrings { get; init; }

        public VarType _VarType { get; init; }
        public string ErrorMessage { get; init; }
        public object Value { get; private set; }

        public bool IsValid(out VarType varType, out string errorMessage)
        {
            if (KeyStrings.Length != 1)
            {
                varType = null;
                errorMessage = $"Unrecognised expression of length {KeyStrings.Length}";
                return false;
            }
            if (KeyStrings[0]._Type == KeyString.Type.Variable)
            {
                Variable variable = Info.DeclaredVariables.Find(var => var.Name == KeyStrings[0].Text);
                if (variable == null)
                {
                    varType = null;
                    errorMessage = $"Variable {KeyStrings[0].Text} hasn't been declared";
                    return false;
                }
                if (!variable.Initialised)
                {
                    varType = null;
                    errorMessage = $"Variable {KeyStrings[0].Text} hasn't been intialised";
                    return false;
                }
                varType = variable._VarType;
                errorMessage = null;
                return true;
            }
            if (KeyStrings[0]._Literal != null)
            {
                Literal literal = KeyStrings[0]._Literal;
                varType = literal._VarType;
                errorMessage = null;
                return true;
            }
            varType = null;
            errorMessage = $"Could not parse key string {KeyStrings[0].Text} in expression";
            return false;
        }

        public class ExpressionException : Exception
        {
            public ExpressionException(Expression expression, string message = null) : base(message)
            {
                this.expression = expression;
            }

            public Expression expression;
        }
    }
}
