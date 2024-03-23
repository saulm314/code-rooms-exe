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

        private IEvaluable resolvedEvaluable;
        private VarType ComputeVarType()
        {
            if (KeyStrings.Length == 0)
                throw new ExpressionException(this, "Cannot have an empty expression");
            int lastTopLevelAllUnitsOperatorIndex = GetLastTopLevelAllUnitsOperatorIndex();
            if (lastTopLevelAllUnitsOperatorIndex != -1)
            {
                Operation @operation = GetOperationFromLastTopLevelAllUnitsOperatorIndex(lastTopLevelAllUnitsOperatorIndex);
                resolvedEvaluable = @operation;
                return resolvedEvaluable._VarType;
            }
            List<int> topLevelOperatorIndexes = GetTopLevelOperatorIndexes();
            RemoveConsecutiveTopLevelOperatorIndexes(topLevelOperatorIndexes);
            ExpressionFrame expressionFrame = GetExpressionFrameFromTopLevelOperatorIndexes(topLevelOperatorIndexes);
            resolvedEvaluable = expressionFrame;
            return resolvedEvaluable._VarType;
        }

        private ExpressionFrame GetExpressionFrameFromTopLevelOperatorIndexes(List<int> topLevelOperatorIndexes)
        {
            AltListLockLock1<IEvaluable, Operator> altExpressionComponents;
            ExpressionUnit firstExpressionUnit;
            if (topLevelOperatorIndexes.Count == 0)
            {
                firstExpressionUnit = new(KeyStrings);
                altExpressionComponents = new(firstExpressionUnit);
                return new(altExpressionComponents);
            }
            firstExpressionUnit = GetFirstExpressionUnitFromFirstTopLevelOperatorIndex(topLevelOperatorIndexes[0]);
            altExpressionComponents = new(firstExpressionUnit);
            FillAltExpressionComponentListUsingTopLevelOperatorIndexes(altExpressionComponents, topLevelOperatorIndexes);
            return new(altExpressionComponents);
        }

        private void FillAltExpressionComponentListUsingTopLevelOperatorIndexes
            (AltListLockLock1<IEvaluable, Operator> altExpressionComponents, List<int> topLevelOperatorIndexes)
        {
            for (int i = 0; i < topLevelOperatorIndexes.Count - 1; i++)
            {
                int currentOperatorIndex = topLevelOperatorIndexes[i];
                int nextOperatorIndex = topLevelOperatorIndexes[i + 1];
                KeyString currentOperatorKeyString = KeyStrings[currentOperatorIndex];
                Operator currentOperator = Operator.GetOperator(currentOperatorKeyString.Text);
                Expression expression = GetExpressionBetweenTwoOperators(currentOperatorIndex, nextOperatorIndex);
                altExpressionComponents.Add(currentOperator, expression);
            }
            int finalOperatorIndex = topLevelOperatorIndexes[topLevelOperatorIndexes.Count - 1];
            KeyString finalOperatorKeyString = KeyStrings[finalOperatorIndex];
            Operator finalOperator = Operator.GetOperator(finalOperatorKeyString.Text);
            Expression finalExpression = GetExpressionAfterFinalOperator(finalOperatorIndex);
            altExpressionComponents.Add(finalOperator, finalExpression);
        }

        private Expression GetExpressionAfterFinalOperator(int finalOperatorIndex)
        {
            if (finalOperatorIndex == KeyStrings.Length - 1)
                return null;
            List<KeyString> currentKeyStrings = new();
            for (int i = finalOperatorIndex; i < KeyStrings.Length; i++)
                currentKeyStrings.Add(KeyStrings[i]);
            return new(currentKeyStrings.ToArray());
        }

        private Expression GetExpressionBetweenTwoOperators(int operator1Index, int operator2Index)
        {
            List<KeyString> currentKeyStrings = new();
            for (int i = operator1Index + 1; i < operator2Index; i++)
                currentKeyStrings.Add(KeyStrings[i]);

            // we don't need to worry about the empty list case
            // because we already removed consecutive operators
            // so the list will never be empty
            return new(currentKeyStrings.ToArray());
        }

        // we assume the list is sorted
        private void RemoveConsecutiveTopLevelOperatorIndexes(List<int> topLevelOperatorIndexes)
        {
            if (topLevelOperatorIndexes.Count <= 1)
                return;
            int i = 1;
            while (i < topLevelOperatorIndexes.Count)
            {
                if (topLevelOperatorIndexes[i] == topLevelOperatorIndexes[i - 1] + 1)
                {
                    topLevelOperatorIndexes.RemoveAt(i);
                    continue;
                }
                i++;
            }
        }

        private ExpressionUnit GetFirstExpressionUnitFromFirstTopLevelOperatorIndex(int firstTopLevelOperatorIndex)
        {
            List<KeyString> currentKeyStrings = new();
            for (int i = 0; i < firstTopLevelOperatorIndex; i++)
            {
                KeyString keyString = KeyStrings[i];
                currentKeyStrings.Add(keyString);
            }
            return currentKeyStrings.Count > 0 ?
                new(currentKeyStrings.ToArray()) :
                null;
        }

        private List<int> GetTopLevelOperatorIndexes()
        {
            List<int> operatorIndexes = new();
            int index = 0;
            int totalBracketsOpened = 0;
            while (index < KeyStrings.Length)
            {
                KeyString keyString = KeyStrings[index];
                bool isOpenBracket = keyString._Type switch
                {
                    KeyString.Type.OpenBracket => true,
                    KeyString.Type.OpenSquareBrace => true,
                    KeyString.Type.OpenCurlyBrace => true,
                    _ => false
                };
                bool isCloseBracket = keyString._Type switch
                {
                    KeyString.Type.CloseBracket => true,
                    KeyString.Type.CloseSquareBrace => true,
                    KeyString.Type.CloseCurlyBrace => true,
                    _ => false
                };
                bool isOperator = keyString._Type == KeyString.Type.Operator;
                if (isOpenBracket)
                {
                    totalBracketsOpened++;
                    index++;
                    continue;
                }
                if (isCloseBracket)
                {
                    totalBracketsOpened--;
                    if (totalBracketsOpened < 0)
                        throw new ExpressionException(this, "Closed bracket without opening it");
                    index++;
                    continue;
                }
                if (isOperator && totalBracketsOpened == 0)
                {
                    operatorIndexes.Add(index);
                    index++;
                    continue;
                }
                index++;
            }
            return operatorIndexes;
        }

        // this refers to all-units operators strictly outside of any type of bracket
        private int GetLastTopLevelAllUnitsOperatorIndex()
        {
            int allUnitsOperatorIndex = -1;
            int index = 0;
            int totalBracketsOpened = 0;
            while (index < KeyStrings.Length)
            {
                KeyString keyString = KeyStrings[index];
                bool isOpenBracket = keyString._Type switch
                {
                    KeyString.Type.OpenBracket => true,
                    KeyString.Type.OpenSquareBrace => true,
                    KeyString.Type.OpenCurlyBrace => true,
                    _ => false
                };
                bool isCloseBracket = keyString._Type switch
                {
                    KeyString.Type.CloseBracket => true,
                    KeyString.Type.CloseSquareBrace => true,
                    KeyString.Type.CloseCurlyBrace => true,
                    _ => false
                };
                bool isAllUnitsOperator =
                    keyString._Type == KeyString.Type.Operator &&
                    keyString._Operator.Priority == OperatorPriority.AllUnits;
                if (isOpenBracket)
                {
                    totalBracketsOpened++;
                    index++;
                    continue;
                }
                if (isCloseBracket)
                {
                    totalBracketsOpened--;
                    if (totalBracketsOpened < 0)
                        throw new ExpressionException(this, "Closed bracket without opening it");
                    index++;
                    continue;
                }
                if (isAllUnitsOperator && totalBracketsOpened == 0)
                {
                    allUnitsOperatorIndex = index;
                    index++;
                    continue;
                }
                index++;
            }
            return allUnitsOperatorIndex;
        }

        private Operation GetOperationFromLastTopLevelAllUnitsOperatorIndex(int lastTopLevelAllUnitsOperatorIndex)
        {
            Expression leftExpression;
            Operator @operator;
            Expression rightExpression;
            List<KeyString> leftKeyStrings = new();
            KeyString operatorKeyString = KeyStrings[lastTopLevelAllUnitsOperatorIndex];
            List<KeyString> rightKeyStrings = new();
            for (int i = 0; i < lastTopLevelAllUnitsOperatorIndex; i++)
                leftKeyStrings.Add(KeyStrings[i]);
            for (int i = lastTopLevelAllUnitsOperatorIndex; i < KeyStrings.Length; i++)
                rightKeyStrings.Add(KeyStrings[i]);
            if (leftKeyStrings.Count == 0)
                throw new ExpressionException(this,
                    $"Must have an expression unit before operator {operatorKeyString.Text}");
            if (rightKeyStrings.Count == 0)
                throw new ExpressionException(this,
                    $"Must have an expression unit after operator {operatorKeyString.Text}");
            leftExpression = new(leftKeyStrings.ToArray());
            @operator = Operator.GetOperator(operatorKeyString.Text);
            rightExpression = new(rightKeyStrings.ToArray());
            return new(leftExpression, @operator, rightExpression);
        }

        public void Compute()
        {
            resolvedEvaluable.Compute();
            Value = resolvedEvaluable.Value;
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
