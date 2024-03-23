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

        public static Operation GetOperation(AltListLockLock1<IEvaluable, Operator> altExpressionComponents, int operatorIndex)
        {
            IEvaluable leftEvaluable = (IEvaluable)altExpressionComponents[operatorIndex - 1];
            Operator @operator = (Operator)altExpressionComponents[operatorIndex];
            IEvaluable rightEvaluable = (IEvaluable)altExpressionComponents[operatorIndex + 1];
            switch (@operator.Priority)
            {
                case OperatorPriority.ImmediateUnits:
                    return new(leftEvaluable, @operator, rightEvaluable);
                case OperatorPriority.LeftToRight:
                    if (operatorIndex != 1)
                        return null;
                    if (altExpressionComponents.Count < operatorIndex + 3)
                        return new(leftEvaluable, @operator, rightEvaluable);
                    Operator nextOperator = (Operator)altExpressionComponents[operatorIndex + 2];
                    if (nextOperator.Priority == OperatorPriority.ImmediateUnits)
                        return null;
                    return new(leftEvaluable, @operator, rightEvaluable);
                case OperatorPriority.AllUnits:
                    if (operatorIndex != 1)
                        return null;
                    AltListLockLock1<IEvaluable, Operator> remainingAltExpressionComponents;
                    int endIndex = altExpressionComponents.Count;
                    for (int i = operatorIndex + 2; i < altExpressionComponents.Count; i += 2)
                        if (((Operator)altExpressionComponents[i]).Priority == OperatorPriority.AllUnits)
                        {
                            endIndex = i;
                            break;
                        }
                    remainingAltExpressionComponents = altExpressionComponents.Sublist(2, endIndex);
                    rightEvaluable = new ExpressionFrame(remainingAltExpressionComponents);
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
