using System;
using System.Collections.Generic;
using CRECSharpInterpreter.Operators;
using CRECSharpInterpreter.Collections.Generic;

namespace CRECSharpInterpreter
{
    public class Operation : IEvaluable
    {
        private Operation(IEvaluable leftEvaluable, Operator @operator, IEvaluable rightEvaluable)
        {
            LeftEvaluable = leftEvaluable;
            _Operator = @operator;
            RightEvaluable = rightEvaluable;

            _VarType = ComputeVarType(out ISpecificOperator specificOperator);
            SpecificOperator = specificOperator;
        }

        public Operation(Expression leftExpression, Operator @operator, Expression rightExpression)
            : this((IEvaluable)leftExpression, @operator, rightExpression) { }

        public IEvaluable LeftEvaluable { get; init; }
        public Operator _Operator { get; init; }
        public IEvaluable RightEvaluable { get; init; }

        public VarType _VarType { get; init; }
        public object Value { get; private set; }

        public ISpecificOperator SpecificOperator { get; init; }

        private VarType ComputeVarType(out ISpecificOperator specificOperator)
        {
            VarType leftType = LeftEvaluable?._VarType;
            VarType rightType = RightEvaluable?._VarType;
            specificOperator = _Operator.GetSpecificOperator(leftType, rightType);
            if (specificOperator == null)
                throw new OperationException(this,
                    $"Invalid operation {_Operator.Symbol} on expressions of type {LeftEvaluable._VarType} and {RightEvaluable._VarType}");
            return specificOperator.ReturnType;
        }

        public void Compute()
        {
            LeftEvaluable?.Compute();
            RightEvaluable?.Compute();
            Value = SpecificOperator.Calculate(LeftEvaluable?.Value, RightEvaluable?.Value);
        }

        // returns null if impossible to determine from the evaluables immediately next to the operator
        // e.g. for the expression "5 * 2 + 3 * 7", if the operatorIndex is 3 (i.e. corresponds to the "+")
        // then an operation cannot be provided because its data type or value cannot be computed
        //      from the evaluables immediately next to the operator
        // that is, just knowing "2 + 3" is not sufficient to provide an operation,
        //      because the "2" is part of a bigger expression ("5 * 2"),
        //      and likewise the "3" is part of a bigger expression ("3 * 7")
        // once the bigger expressions are resolved and given as a single evaluable,
        // then this method can provide an operation
        //
        // if this method is fed any all-units operators, it throws an error
        // to get an operation involving all-units operators, use the public constructor
        public static Operation GetOperation(AltListLockLock1<IEvaluable, Operator> altExpressionComponents, int operatorIndex)
        {
            if (altExpressionComponents.Count < 3)
                throw new OperationException(null, "Internal error: cannot create an operation with fewer than 3 expression components");
            IEvaluable leftEvaluable = (IEvaluable)altExpressionComponents[operatorIndex - 1];
            Operator @operator = (Operator)altExpressionComponents[operatorIndex];
            IEvaluable rightEvaluable = (IEvaluable)altExpressionComponents[operatorIndex + 1];
            if (@operator.Priority == OperatorPriority.ImmediateUnits)
                return new(leftEvaluable, @operator, rightEvaluable);
            if (@operator.Priority != OperatorPriority.LeftToRight)
                throw new OperationException(null, $"Internal error: unexpected operator priority {@operator.Priority}\n" +
                    $"To get an operation using this operator use the public constructor");
            
            // operator priority is LeftToRight
            Operator previousOperator =
                operatorIndex >= 3 ?
                (Operator)altExpressionComponents[operatorIndex - 2] :
                null;
            bool previousOperatorIsImmediateUnits = previousOperator?.Priority == OperatorPriority.ImmediateUnits;
            Operator nextOperator =
                operatorIndex <= altExpressionComponents.Count - 4 ?
                (Operator)altExpressionComponents[operatorIndex + 2] :
                null;
            bool nextOperatorIsImmediateUnits = previousOperator?.Priority == OperatorPriority.ImmediateUnits;
            if (previousOperatorIsImmediateUnits || nextOperatorIsImmediateUnits)
                return null;
            return new(leftEvaluable, @operator, rightEvaluable);
        }

        public KeyString[] GetKeyStrings()
        {
            if (_keyStrings != null)
                return _keyStrings;
            KeyString[] leftKeyStrings = LeftEvaluable?.GetKeyStrings() ?? Array.Empty<KeyString>();
            KeyString operatorKeyString = _Operator.KeyString;
            KeyString[] rightKeyStrings = RightEvaluable?.GetKeyStrings() ?? Array.Empty<KeyString>();
            KeyString[] keyStrings = new KeyString[leftKeyStrings.Length + 1 + rightKeyStrings.Length];
            leftKeyStrings.CopyTo(keyStrings, 0);
            keyStrings[leftKeyStrings.Length] = operatorKeyString;
            rightKeyStrings.CopyTo(keyStrings, leftKeyStrings.Length + 1);
            return _keyStrings = keyStrings;
        }

        private KeyString[] _keyStrings;

        public override string ToString()
        {
            string str = string.Empty;
            foreach (KeyString keyString in GetKeyStrings())
                str += keyString.Text;
            return str;
        }

        public class OperationException : InterpreterException
        {
            public OperationException(Operation operation, string message = null) : base(message)
            {
                this.operation = operation;
            }

            public Operation operation;
        }
    }
}
