using System;
using System.Collections.Generic;

namespace CRECSharpInterpreter
{
    public class Operation : IEvaluable
    {
        private Operation(IEvaluable leftEvaluable, Operator @operator, IEvaluable rightEvaluable)
        {
            LeftEvaluable = leftEvaluable;
            _Operator = @operator;
            RightEvaluable = rightEvaluable;

            _VarType = ComputeVarType(out IOperator specificOperator);
            SpecificOperator = specificOperator;
        }

        public IEvaluable LeftEvaluable { get; init; }
        public Operator _Operator { get; init; }
        public IEvaluable RightEvaluable { get; init; }

        public VarType _VarType { get; init; }
        public object Value { get; private set; }

        public IOperator SpecificOperator { get; init; }

        private VarType ComputeVarType(out IOperator specificOperator)
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

        public static Operation GetOperation(List<IEvaluable> evaluables, List<Operator> operators, int operatorIndex)
        {
            if (operators.Count == 0)
                throw new OperationException(null, "internal error");
            if (evaluables.Count != operators.Count + 1)
                throw new OperationException(null, "internal error");
            IEvaluable leftEvaluable = evaluables[operatorIndex];
            IEvaluable rightEvaluable = evaluables[operatorIndex + 1];
            Operator @operator = operators[operatorIndex];
            switch (@operator.Priority)
            {
                case OperatorPriority.ImmediateExpressions:
                    return new(leftEvaluable, @operator, rightEvaluable);
                case OperatorPriority.AllExpressions:
                    if (evaluables.Count != 2)
                        return null;
                    return new (leftEvaluable, @operator, rightEvaluable);
                case OperatorPriority.LeftToRight:
                    if (operatorIndex != 0)
                        return null;
                    if (operators.Count == 1)
                        return new(leftEvaluable, @operator, rightEvaluable);
                    Operator nextOperator = operators[operatorIndex + 1];
                    if (nextOperator.Priority == OperatorPriority.ImmediateExpressions)
                        return null;
                    return new(leftEvaluable, @operator, rightEvaluable);
                default:
                    throw new OperationException(null, "internal error");
            }
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
            return keyStrings;
        }

        private KeyString[] _keyStrings;

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
