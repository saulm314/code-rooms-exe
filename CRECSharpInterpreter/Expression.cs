using System.Collections.Generic;

namespace CRECSharpInterpreter
{
    public class Expression
    {
        public Expression(KeyString[] keyStrings)
        {
            KeyStrings = keyStrings;
            VerifyVariablesAreInitialised();
            _VarType = ComputeVarType();
        }

        public KeyString[] KeyStrings { get; init; }

        public VarType _VarType { get; init; }
        public object Value { get; private set; }
        public Type _Type { get; private set; }

        private void VerifyVariablesAreInitialised()
        {
            foreach (KeyString keyString in KeyStrings)
                if (keyString._Type == KeyString.Type.Variable)
                {
                    Variable variable = Memory.Instance.GetVariable(keyString.Text);
                    if (variable == null)
                        throw new ExpressionException(this, $"Variable \"{keyString.Text}\" hasn't been declared");
                    if (!variable.Initialised)
                        throw new ExpressionException(this, $"Variable \"{keyString.Text}\" hasn't been initialised");
                }
        }

        private ExpressionException lengthException;
        private VarType ComputeVarType()
        {
            lengthException = new(this, $"Unrecognised expression of length {KeyStrings.Length}");
            _Type = GetType();
            return _Type switch
            {
                Type.Variable => ComputeVarTypeVariable(),
                Type.Literal => ComputeVarTypeLiteral(),
                Type.ArrayConstruction => ComputeVarTypeArrayConstruction(),
                Type.ArrayLiteral => ComputeVarTypeArrayLiteral(),
                _ => throw lengthException
            };
        }

        private new Type GetType()
        {
            if (KeyStrings.Length == 0)
                return Type.Invalid;
            if (KeyStrings[0]._Type == KeyString.Type.Variable && KeyStrings.Length == 1)
                return Type.Variable;
            if (KeyStrings[0]._Literal != null && KeyStrings.Length == 1)
                return Type.Literal;
            if (KeyStrings[0]._Type == KeyString.Type.NewKeyword)
            {
                if (KeyStrings.Length < 2)
                    return Type.Invalid;
                if (KeyStrings[1]._Type == KeyString.Type.ArrayConstruction && KeyStrings.Length <= 2)
                    return Type.ArrayConstruction;
                if (KeyStrings[1]._Type == KeyString.Type.Type && KeyStrings.Length >= 4)
                    return Type.ArrayLiteral;
            }
            return Type.Invalid;
        }

        private VarType ComputeVarTypeVariable()
        {
            if (KeyStrings.Length != 1)
                throw lengthException;
            Variable variable = Memory.Instance.GetVariable(KeyStrings[0].Text);
            return variable._VarType;
        }

        private VarType ComputeVarTypeLiteral()
        {
            if (KeyStrings.Length != 1)
                throw lengthException;
            Literal literal = KeyStrings[0]._Literal;
            return literal._VarType;
        }

        private VarType ComputeVarTypeArrayConstruction()
        {
            if (KeyStrings.Length > 2)
                throw lengthException;
            return KeyStrings[1]._ArrayConstruction._VarType;
        }

        private VarType ComputeVarTypeArrayLiteral()
        {
            if (KeyStrings.Length < 4)
                throw lengthException;
            string varTypeAsString = KeyStrings[1].Text;
            VarType varType = VarType.GetVarType(varTypeAsString);
            if (KeyStrings[2]._Type != KeyString.Type.OpenCurlyBrace)
                throw lengthException;
            if (KeyStrings[KeyStrings.Length - 1]._Type != KeyString.Type.CloseCurlyBrace)
                throw lengthException;
            List<Expression> expressionsInsideBraces = GetExpressionsInArrayLiteral();
            foreach (Expression expression in expressionsInsideBraces)
                if (expression._VarType != varType.Unarray)
                    throw new ExpressionException(this,
                        $"Cannot have variable of type {expression._VarType} in array of type {varType}");
            return varType;
        }

        public void Compute()
        {
            switch (_Type)
            {
                case Type.Variable:
                    ComputeVariable();
                    break;
                case Type.Literal:
                    ComputeLiteral();
                    break;
                case Type.ArrayConstruction:
                    ComputeArrayConstruction();
                    break;
                case Type.ArrayLiteral:
                    ComputeArrayLiteral();
                    break;
                default:
                    throw new ExpressionException(this,
                        $"Internal exception: don't know how to compute expression of type {_Type}");
            }
        }

        private void ComputeVariable()
        {
            Variable variable = Memory.Instance.GetVariable(KeyStrings[0].Text);
            Value = variable.Value;
        }

        private void ComputeLiteral()
        {
            Value = KeyStrings[0]._Literal.Value;
        }

        private void ComputeArrayConstruction()
        {
            ArrayConstruction arrayConstruction = KeyStrings[1]._ArrayConstruction;
            arrayConstruction.ArrayLengthExpression.Compute();
            arrayConstruction.ArrayLength = (int)arrayConstruction.ArrayLengthExpression.Value;
            IEnumerable<Variable> variables = Variable.GetBlankVariables(arrayConstruction._VarType.Unarray, arrayConstruction.ArrayLength);
            int heapIndex = Memory.Instance.Heap.Allocate(arrayConstruction.ArrayLength, variables);
            Value = heapIndex;
        }

        private void ComputeArrayLiteral()
        {
            List<Expression> expressionsInsideBraces = GetExpressionsInArrayLiteral();
            foreach (Expression expression in expressionsInsideBraces)
                expression.Compute();
            Variable[] variablesToAllocate = new Variable[expressionsInsideBraces.Count];
            for (int i = 0; i < variablesToAllocate.Length; i++)
            {
                variablesToAllocate[i] = new(_VarType.Unarray);
                variablesToAllocate[i].Value = expressionsInsideBraces[i].Value;
            }
            int heapIndex = Memory.Instance.Heap.Allocate(variablesToAllocate.Length, variablesToAllocate);
            Value = heapIndex;
        }

        private List<Expression> GetExpressionsInArrayLiteral()
        {
            List<Expression> expressionsInsideBraces = new();
            List<KeyString> keyStringsInCurrentExpression = new();
            bool expectingNonComma = true;
            for (int i = 3; i < KeyStrings.Length - 1; i++)
            {
                switch (KeyStrings[i]._Type)
                {
                    case KeyString.Type.Comma:
                        if (expectingNonComma)
                            throw lengthException;
                        expectingNonComma = true;
                        Expression currentExpression = new(keyStringsInCurrentExpression.ToArray());
                        expressionsInsideBraces.Add(currentExpression);
                        keyStringsInCurrentExpression = new();
                        break;
                    default:
                        expectingNonComma = false;
                        keyStringsInCurrentExpression.Add(KeyStrings[i]);
                        break;
                }
            }
            if (expectingNonComma)
                throw lengthException;
            Expression finalExpression = new(keyStringsInCurrentExpression.ToArray());
            expressionsInsideBraces.Add(finalExpression);
            return expressionsInsideBraces;
        }

        public enum Type
        {
            Invalid,
            Variable,
            Literal,
            ArrayConstruction,
            ArrayLiteral
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
