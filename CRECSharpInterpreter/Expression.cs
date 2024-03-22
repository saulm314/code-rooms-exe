using System;
using System.Collections.Generic;
using CRECSharpInterpreter.Operators;

namespace CRECSharpInterpreter
{
    public class Expression : IEvaluable
    {
        public Expression(KeyString[] keyStrings)
        {
            KeyStrings = keyStrings;
            _VarType = ComputeVarType();
            if (_Type == Type.Variable)
                VerifyVariableInitialised();
        }

        public KeyString[] KeyStrings { get; init; }

        public KeyString[] GetKeyStrings() => KeyStrings;

        public VarType _VarType { get; init; }
        public object Value { get; private set; }
        public Type _Type { get; private set; }

        private IEvaluable evaluable;

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
                Type.ContainsOperators => ComputeVarTypeContainsOperators(),
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
            if (Array.Exists(KeyStrings, keyString => keyString._Type == KeyString.Type.Operator))
                return Type.ContainsOperators;
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

        private VarType ComputeVarTypeContainsOperators()
        {
            GetEvaluablesSeparatedByOperators(out List<IEvaluable> evaluables, out List<Operator> operators);
            while (operators.Count > 0)
            {
                for (int i = 0; i < operators.Count; i++)
                {
                    Operation operation = Operation.GetOperation(evaluables, operators, i);
                    if (operation != null)
                    {
                        operators.RemoveAt(i);
                        evaluables.RemoveAt(i);
                        evaluables[i] = operation;
                        break;
                    }
                }
            }
            evaluable = evaluables[0];
            return evaluable._VarType;
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
                case Type.ContainsOperators:
                    ComputeContainsOperators();
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

        // there will always be 1 more evaluable than operator
        // this is because it is assumed that the parent expression starts with an expression
        //      (if it in fact does not then we say it starts with a null expression)
        // and similarly it is assumed that the parent expression ends with an expression (same caveat)
        private void GetEvaluablesSeparatedByOperators(out List<IEvaluable> evaluables, out List<Operator> operators)
        {
            internalException = new(this, "internal error");
            evaluables = new();
            operators = new();
            int i = 0;
            ItemType previous = ItemType.None;
            while (i < KeyStrings.Length)
            {
                KeyString[] remainingKeyStrings = new KeyString[KeyStrings.Length - i];
                Array.Copy(KeyStrings, i, remainingKeyStrings, 0, KeyStrings.Length - i);
                IEvaluable evaluable = GetNextEvaluable(ref previous, i, out i);
                evaluables.Add(evaluable);
                if (i >= KeyStrings.Length)
                    break;
                Operator @operator = GetNextOperator(ref previous, i, out i);
                operators.Add(@operator);
            }
            switch (previous)
            {
                case ItemType.Evaluable:
                    break;
                case ItemType.AllExpressionsOperator:
                case ItemType.Operator_NotAllExpressions:
                    evaluables.Add(null);
                    break;
            }
        }

        private ExpressionException internalException;

        private IEvaluable GetNextEvaluable(ref ItemType previous, int startIndex, out int endIndex)
        {
            endIndex = startIndex + 1;

            bool isOperator = KeyStrings[startIndex]._Type == KeyString.Type.Operator;
            switch (previous, isOperator)
            {
                case (ItemType.None, false):
                case (ItemType.AllExpressionsOperator, false):
                case (ItemType.Operator_NotAllExpressions, false):
                    previous = ItemType.Evaluable;
                    break;
                case (ItemType.None, true):
                case (ItemType.AllExpressionsOperator, true):
                    previous = ItemType.Evaluable;
                    endIndex--;
                    return null;
                case (ItemType.Evaluable, false):
                case (ItemType.Evaluable, true):
                    throw internalException;
                case (ItemType.Operator_NotAllExpressions, true):
                    if (KeyStrings[startIndex]._Operator.Priority == OperatorPriority.AllExpressions)
                    {
                        previous = ItemType.Evaluable;
                        endIndex--;
                        return null;
                    }
                    throw new ExpressionException(this,
                        $"Cannot have operator {KeyStrings[startIndex]._Operator.Symbol} immediately after another operator");
            }

            List<KeyString> keyStringsInExpression = new();
            int i = startIndex;
            while (i < KeyStrings.Length)
            {
                switch (KeyStrings[i]._Type)
                {
                    default:
                        keyStringsInExpression.Add(KeyStrings[i]);
                        break;
                    case KeyString.Type.Operator:
                        endIndex = i;
                        return new Expression(keyStringsInExpression.ToArray());
                }
                i++;
            }
            endIndex = i;
            return new Expression(keyStringsInExpression.ToArray());
        }

        private Operator GetNextOperator(ref ItemType previous, int startIndex, out int endIndex)
        {
            endIndex = startIndex + 1;

            if (previous != ItemType.Evaluable)
                throw internalException;
            if (KeyStrings[startIndex]._Type != KeyString.Type.Operator)
                throw internalException;

            previous = KeyStrings[startIndex]._Operator.Priority == OperatorPriority.AllExpressions ?
                ItemType.AllExpressionsOperator :
                ItemType.Operator_NotAllExpressions;

            return KeyStrings[startIndex]._Operator;
        }

        private enum ItemType
        {
            None,
            Evaluable,
            AllExpressionsOperator,
            Operator_NotAllExpressions
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

        private void ComputeContainsOperators()
        {
            evaluable.Compute();
            Value = evaluable.Value;
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
            ArrayLength,
            ContainsOperators
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
