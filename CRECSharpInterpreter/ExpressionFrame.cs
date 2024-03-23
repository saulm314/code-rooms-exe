using System.Collections.Generic;
using CRECSharpInterpreter.Collections.Generic;
using CRECSharpInterpreter.Operators;

namespace CRECSharpInterpreter
{
    public class ExpressionFrame : IEvaluable
    {
        public ExpressionFrame(AltListLockLock1<IEvaluable, Operator> altExpressionComponents)
        {
            AltExpressionComponents = altExpressionComponents;
            _VarType = ComputeVarType();
        }

        public AltListLockLock1<IEvaluable, Operator> AltExpressionComponents { get; init; }

        public VarType _VarType { get; init; }
        public object Value { get; private set; }

        public KeyString[] GetKeyStrings()
        {
            List<KeyString> keyStrings = new();
            foreach (IExpressionComponent component in AltExpressionComponents)
                foreach (KeyString keyString in component.GetKeyStrings())
                    keyStrings.Add(keyString);
            return keyStrings.ToArray();
        }

        private IEvaluable resolvedEvaluable;
        private VarType ComputeVarType()
        {
            while (AltExpressionComponents.Count > 1)
            {
                for (int i = 1; i < AltExpressionComponents.Count; i += 2)
                {
                    Operation operation = Operation.GetOperation(AltExpressionComponents, i);
                    if (operation != null)
                    {
                        AltExpressionComponents.RemoveAt(i);
                        AltExpressionComponents[i - 1] = operation;
                        break;
                    }
                }
            }
            resolvedEvaluable = (IEvaluable)AltExpressionComponents[0];
            return resolvedEvaluable._VarType;
        }

        public void Compute()
        {
            resolvedEvaluable.Compute();
            Value = resolvedEvaluable.Value;
        }
    }
}
