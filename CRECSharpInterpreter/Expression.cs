using System;
using System.Collections.Generic;

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
            ExpressionException lengthException = new(this, $"Unrecognised expression of length {KeyStrings.Length}");
            if (KeyStrings.Length < 1 || KeyStrings.Length > 2)
                throw lengthException;
            if (KeyStrings[0]._Type == KeyString.Type.Variable)
            {
                if (KeyStrings.Length != 1)
                    throw lengthException;
                Variable variable = Memory.Instance.GetVariable(KeyStrings[0].Text);
                return variable._VarType;
            }
            if (KeyStrings[0]._Literal != null)
            {
                if (KeyStrings.Length != 1)
                    throw lengthException;
                Literal literal = KeyStrings[0]._Literal;
                return literal._VarType;
            }
            if (KeyStrings.Length < 2)
                throw lengthException;
            if (KeyStrings[0]._Type == KeyString.Type.NewKeyword && KeyStrings[1]._Type == KeyString.Type.ArrayConstruction)
            {
                if (KeyStrings.Length > 2)
                    throw lengthException;
                ArrayConstruction arrayConstruction = KeyStrings[1]._ArrayConstruction;
                return arrayConstruction._VarType;
            }
            throw new ExpressionException(this, $"Could not parse key string {KeyStrings[0]} in expression");
        }

        public void Compute()
        {
            if (KeyStrings[0]._Type == KeyString.Type.Variable)
            {
                Variable variable = Memory.Instance.GetVariable(KeyStrings[0].Text);
                Value = variable.Value;
                return;
            }
            if (KeyStrings[0]._Literal != null)
            {
                Value = KeyStrings[0]._Literal.Value;
                return;
            }
            if (KeyStrings[0]._Type == KeyString.Type.NewKeyword && KeyStrings[1]._Type == KeyString.Type.ArrayConstruction)
            {
                ArrayConstruction arrayConstruction = KeyStrings[1]._ArrayConstruction;
                IEnumerable<Variable> variables = Variable.GetBlankVariables(arrayConstruction._VarType.Unarray, arrayConstruction.ArrayLength);
                int heapIndex = Memory.Instance.Heap.Allocate(arrayConstruction.ArrayLength, variables);
                Value = heapIndex;
                return;
            }
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
