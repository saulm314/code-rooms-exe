using System.Collections.Generic;

namespace CRECSharpInterpreter
{
    public class Expression
    {
        public Expression(KeyString[] keyStrings)
        {
            KeyStrings = keyStrings;
            _VarType = ComputeVarType();
            if (_Type == Type.Variable)
                VerifyVariableInitialised();
        }

        public KeyString[] KeyStrings { get; init; }

        public VarType _VarType { get; init; }
        public object Value { get; private set; }
        public Type _Type { get; private set; }

        private void VerifyVariableInitialised()
        {
            Variable variable = Memory.Instance.GetVariable(KeyStrings[0].Text);
            if (variable == null)
                throw new ExpressionException(this, $"Variable \"{KeyStrings[0].Text}\" hasn't been declared");
            if (!variable.Initialised)
                throw new ExpressionException(this, $"Variable \"{KeyStrings[0].Text}\" hasn't been initialised");
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
                Type.ArrayElement => ComputeVarTypeArrayElement(),
                Type.Null => ComputeVarTypeNull(),
                Type.ArrayLength => ComputeVarTypeArrayLength(),
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
            if (KeyStrings[0]._Type == KeyString.Type.ArrayElement && KeyStrings.Length == 1)
                return Type.ArrayElement;
            if (KeyStrings[0]._Type == KeyString.Type.Null && KeyStrings.Length == 1)
                return Type.Null;
            if (KeyStrings[0]._Type == KeyString.Type.ArrayLength && KeyStrings.Length == 1)
                return Type.ArrayLength;
            return Type.Invalid;
        }

        private VarType ComputeVarTypeVariable()
        {
            Variable variable = Memory.Instance.GetVariable(KeyStrings[0].Text);
            return variable._VarType;
        }

        private VarType ComputeVarTypeLiteral()
        {
            Literal literal = KeyStrings[0]._Literal;
            return literal._VarType;
        }

        private VarType ComputeVarTypeArrayConstruction()
        {
            return KeyStrings[1]._ArrayConstruction._VarType;
        }

        private VarType ComputeVarTypeNull()
        {
            return null;
        }

        private VarType ComputeVarTypeArrayLiteral()
        {
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

        private VarType ComputeVarTypeArrayElement()
        {
            return KeyStrings[0]._ArrayElement.Array._VarType.Unarray;
        }

        private VarType ComputeVarTypeArrayLength()
        {
            return VarType.@int;
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
                case Type.ArrayElement:
                    ComputeArrayElement();
                    break;
                case Type.Null:
                    ComputeNull();
                    break;
                case Type.ArrayLength:
                    ComputeArrayLength();
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
            if (variable._VarType._Storage == VarType.Storage.Value)
                return;
            if (Value == null)
                return;
            // if it's a reference type with a non-null value then its value is the heap index
            int heapIndex = (int)Value;
            Memory.Instance.Heap.IncrementReferenceCounter(heapIndex);
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
            if (KeyStrings.Length == 4)
                return new();
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

        private void ComputeArrayElement()
        {
            Expression indexExpression = KeyStrings[0]._ArrayElement.IndexExpression;
            indexExpression.Compute();
            KeyStrings[0]._ArrayElement.Index = (int)indexExpression.Value;
            int index = KeyStrings[0]._ArrayElement.Index;
            Variable array = KeyStrings[0]._ArrayElement.Array;
            if (array.Value == null)
                throw new ExpressionException(this, $"Array \"{array.Name}\" has value null");
            int heapIndex = (int)array.Value;
            int length = (int)Memory.Instance.Heap[heapIndex].Value;
            if (index >= length)
                throw new ExpressionException(this, $"Index {index} out of range of array \"{array.Name}\"");
            Value = Memory.Instance.Heap.GetValue(heapIndex, index);
        }

        private void ComputeNull()
        {
            Value = null;
        }

        private void ComputeArrayLength()
        {
            ArrayLength arrayLength = KeyStrings[0]._ArrayLength;
            Value = arrayLength.Length;
        }

        public enum Type
        {
            Invalid,
            Variable,
            Literal,
            ArrayConstruction,
            ArrayLiteral,
            ArrayElement,
            Null,
            ArrayLength
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
