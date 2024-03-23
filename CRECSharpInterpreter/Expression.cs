using System.Collections.Generic;
using CRECSharpInterpreter.Collections.Generic;
using CRECSharpInterpreter.Operators;

namespace CRECSharpInterpreter
{
    public class Expression : IEvaluable
    {
        public Expression(KeyString[] keyStrings)
        {
            KeyStrings = keyStrings;
            _VarType = ComputeVarType();
        }

        public KeyString[] KeyStrings { get; init; }

        public KeyString[] GetKeyStrings() => KeyStrings;

        public VarType _VarType { get; init; }
        public object Value { get; private set; }

        private ExpressionFrame expressionFrame;
        private VarType ComputeVarType()
        {
            AltListLockLock1<IEvaluable, Operator> altExpressionComponents;
            ExpressionUnit firstExpressionUnit = GetFirstExpressionUnit(out int operatorIndex);
            altExpressionComponents = new(firstExpressionUnit);
            if (operatorIndex < KeyStrings.Length)
                FillAltExpressionComponents(altExpressionComponents, operatorIndex);
            expressionFrame = new(altExpressionComponents);
            return expressionFrame._VarType;
        }

        private ExpressionUnit GetFirstExpressionUnit(out int operatorIndex)
        {
            List<KeyString> currentKeyStrings = new();
            operatorIndex = 0;
            bool curlyBraceOpened = false;
            foreach (KeyString keyString in KeyStrings)
            {
                if (keyString._Type == KeyString.Type.OpenCurlyBrace)
                    curlyBraceOpened = true;
                if (keyString._Type == KeyString.Type.CloseCurlyBrace)
                    curlyBraceOpened = false;
                if (keyString._Type == KeyString.Type.Operator && !curlyBraceOpened)
                    return currentKeyStrings.Count > 0 ?
                        new(currentKeyStrings.ToArray()) :
                        null;
                currentKeyStrings.Add(keyString);
                operatorIndex++;
            }
            return new(currentKeyStrings.ToArray());
        }

        private void FillAltExpressionComponents(AltListLockLock1<IEvaluable, Operator> altExpressionComponents, int firstOperatorIndex)
        {
            foreach (Pair<Operator, IEvaluable> pair in GetPairs(altExpressionComponents, firstOperatorIndex))
                altExpressionComponents.Add(pair);
        }

        private IEnumerable<Pair<Operator, IEvaluable>> GetPairs(AltListLockLock1<IEvaluable, Operator> altExpressionComponents, int firstOperatorIndex)
        {
            List<KeyString> currentKeyStrings = new();
            Pair<Operator, IEvaluable> pair = new(default, default);
            bool firstOperatorProcessed = false;
            int i = firstOperatorIndex;
            while (i < KeyStrings.Length)
            {
                if (KeyStrings[i]._Type == KeyString.Type.Operator)
                {
                    KeyString keyString = KeyStrings[i];
                    if (!firstOperatorProcessed)
                    {
                        firstOperatorProcessed = true;
                        pair.First = keyString._Operator;
                        i++;
                        continue;
                    }
                    ExpressionUnit expressionUnit = currentKeyStrings.Count > 0 ?
                        new(currentKeyStrings.ToArray()) :
                        null;
                    currentKeyStrings = new();
                    IEvaluable evaluable = ProcessOperator(expressionUnit, ref i);
                    pair.Second = evaluable;
                    yield return pair;
                    pair = new(keyString._Operator, default);
                    continue;
                }
                currentKeyStrings.Add(KeyStrings[i]);
                i++;
            }
            KeyString finalKeyString = KeyStrings[i - 1];
            bool finalWasOperator = finalKeyString._Type == KeyString.Type.Operator;
            if (finalWasOperator)
            {
                pair.Second = null;
                yield return pair;
                yield break;
            }
            ExpressionUnit finalUnit = new(currentKeyStrings.ToArray());
            pair.Second = finalUnit;
            yield return pair;
        }

        private IEvaluable ProcessOperator(ExpressionUnit expressionUnit, ref int index)
        {
            if (expressionUnit != null)
            {
                index++;
                return expressionUnit;
            }

            // there must have been two operators in a row
            Operator previousOperator = KeyStrings[index - 1]._Operator;
            Operator currentOperator = KeyStrings[index]._Operator;

            bool previousIsAllUnits = previousOperator.Priority == OperatorPriority.AllUnits;
            bool currentIsAllUnits = currentOperator.Priority == OperatorPriority.AllUnits;
            bool exactlyOneIsAllUnits = previousIsAllUnits ^ currentIsAllUnits;
            if (exactlyOneIsAllUnits)
            {
                index++;
                return null;
            }

            bool bothAreAllUnits = previousIsAllUnits && currentIsAllUnits;
            if (bothAreAllUnits)
                throw new ExpressionException(this, "Cannot have two all-units operators in a row");

            // both are strictly non-AllUnits
            return GetSubexpression(ref index);
        }

        // keep going until we hit:
        //      1) the end of an expression unit (isOperator && expressionUnitHit)
        //      2) all-units operator && !expressionUnitHit (error)
        //      3) the end && !expressionUnitHit (error)
        //      4) the end && expressionUnitHit
        private Expression GetSubexpression(ref int index)
        {
            List<KeyString> currentKeyStrings = new();
            bool expressionUnitHit = false;
            while (index < KeyStrings.Length)
            {
                KeyString keyString = KeyStrings[index];
                index++;
                bool isOperator = keyString._Type == KeyString.Type.Operator;
                bool isAllUnitsOperator =
                    isOperator &&
                    keyString._Operator.Priority == OperatorPriority.AllUnits;
                if (isAllUnitsOperator && !expressionUnitHit)
                    throw new ExpressionException(this, "Cannot have an all-units operator after a sequence of operators");
                if (isOperator && expressionUnitHit)
                    return new(currentKeyStrings.ToArray());
                currentKeyStrings.Add(keyString);
                if (!isOperator)
                    expressionUnitHit = true;
            }
            if (!expressionUnitHit)
                throw new ExpressionException(this, "Cannot end expression in a sequence of operators");
            return new(currentKeyStrings.ToArray());
        }

        public void Compute()
        {
            expressionFrame.Compute();
            Value = expressionFrame.Value;
        }

        public override string ToString()
        {
            string str = string.Empty;
            foreach (KeyString keyString in GetKeyStrings())
                str += keyString.Text;
            return str;
        }

        public class ExpressionException : InterpreterException
        {
            public ExpressionException(Expression expression, string message = null) : base(expression.ToString() + " " + message)
            {
                this.expression = expression;
            }

            public Expression expression;
        }
    }
}
