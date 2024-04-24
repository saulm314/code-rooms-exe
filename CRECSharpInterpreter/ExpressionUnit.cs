using System;
using System.Collections.Generic;

namespace CRECSharpInterpreter
{
    public class ExpressionUnit : IEvaluable
    {
        public ExpressionUnit(KeyString[] keyStrings)
        {
            KeyStrings = keyStrings;
            _VarType = ComputeVarType();
        }

        public KeyString[] KeyStrings { get; init; }

        public KeyString[] GetKeyStrings() => KeyStrings;

        public VarType? _VarType { get; init; }
        public object? Value { get; private set; }
        public Type _Type { get; private set; }

        private VarType? ComputeVarType()
        {
            _Type = GetType();
            return _Type switch
            {
                Type.Variable =>            ComputeVarTypeVariable(),
                Type.Literal =>             ComputeVarTypeLiteral(),
                Type.ArrayConstruction =>   ComputeVarTypeArrayConstruction(),
                Type.ArrayLiteral =>        ComputeVarTypeArrayLiteral(),
                Type.ArrayElement =>        ComputeVarTypeArrayElement(),
                Type.Null =>                ComputeVarTypeNull(),
                Type.ArrayLength =>         ComputeVarTypeArrayLength(),
                Type.Bracket =>             ComputeVarTypeBracket(),
                Type.StringLiteral =>       ComputeVarTypeStringLiteral(),
                Type.StringElement =>       ComputeVarTypeStringElement(),
                Type.ArrayStringElement =>  ComputeVarTypeArrayStringElement(),
                Type.StringLength =>        ComputeVarTypeStringLength(),
                Type.StringConstruction =>  ComputeVarTypeStringConstruction(),
                _ =>                        throw new ExpressionUnitException(this, "Unrecognised expression unit")
            };
        }

        public void Compute()
        {
            switch (_Type)
            {
                case Type.Variable:             ComputeVariable();              break;
                case Type.Literal:              ComputeLiteral();               break;
                case Type.ArrayConstruction:    ComputeArrayConstruction();     break;
                case Type.ArrayLiteral:         ComputeArrayLiteral();          break;
                case Type.ArrayElement:         ComputeArrayElement();          break;
                case Type.Null:                 ComputeNull();                  break;
                case Type.ArrayLength:          ComputeArrayLength();           break;
                case Type.Bracket:              ComputeBracket();               break;
                case Type.StringLiteral:        ComputeStringLiteral();         break;
                case Type.StringElement:        ComputeStringElement();         break;
                case Type.ArrayStringElement:   ComputeArrayStringElement();    break;
                case Type.StringLength:         ComputeStringLength();          break;
                case Type.StringConstruction:   ComputeStringConstruction();    break;
                default:                        throw new ExpressionUnitException(this,
                                                    $"Internal error; don't know how to compute expression of type \"{_Type}\"");
            }
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
                if (KeyStrings[1]._Type == KeyString.Type.ArrayConstruction && KeyStrings.Length == 2)
                    return Type.ArrayConstruction;
                if
                (
                    KeyStrings[1]._Type == KeyString.Type.Type &&
                    VarType.GetVarType(KeyStrings[1].Text)!.IsArray &&
                    KeyStrings.Length >= 4
                )
                    return Type.ArrayLiteral;
                if
                (
                    KeyStrings[1]._Type == KeyString.Type.Type &&
                    VarType.GetVarType(KeyStrings[1].Text) == VarType.@string &&
                    KeyStrings[2]._Type == KeyString.Type.OpenBracket &&
                    KeyStrings[KeyStrings.Length - 1]._Type == KeyString.Type.CloseBracket &&
                    KeyStrings.Length >= 5
                )
                    return Type.StringConstruction;
            }
            if (KeyStrings[0]._Type == KeyString.Type.ArrayElement && KeyStrings.Length == 1)
                return Type.ArrayElement;
            if (KeyStrings[0]._Type == KeyString.Type.StringElement && KeyStrings.Length == 1)
                return Type.StringElement;
            if (KeyStrings[0]._Type == KeyString.Type.ArrayStringElement && KeyStrings.Length == 1)
                return Type.ArrayStringElement;
            if (KeyStrings[0]._Type == KeyString.Type.Null && KeyStrings.Length == 1)
                return Type.Null;
            if (KeyStrings[0]._Type == KeyString.Type.ArrayLength && KeyStrings.Length == 1)
                return Type.ArrayLength;
            if (KeyStrings[0]._Type == KeyString.Type.StringLength && KeyStrings.Length == 1)
                return Type.StringLength;
            if (KeyStrings[0]._Type == KeyString.Type.OpenBracket && KeyStrings[KeyStrings.Length - 1]._Type == KeyString.Type.CloseBracket)
                return Type.Bracket;
            if (KeyStrings[0]._Type == KeyString.Type.StringLiteral && KeyStrings.Length == 1)
                return Type.StringLiteral;
            return Type.Invalid;
        }

        private Variable? variable;
        private VarType? ComputeVarTypeVariable()
        {
            variable = Memory.Instance!.GetVariable(KeyStrings[0].Text);
            if (variable == null)
                throw new ExpressionUnitException(this, $"Variable \"{KeyStrings[0].Text}\" hasn't been declared");
            if (!variable.Initialised)
                throw new ExpressionUnitException(this, $"Variable \"{KeyStrings[0].Text}\" hasn't been initialised");
            return variable._VarType;
        }

        private void ComputeVariable()
        {
            Value = variable!.Value;
            if (variable._VarType!._Storage == VarType.Storage.Value)
                return;
            if (Value == null)
                return;
            // if it's a reference type with a non-null value then its value is the heap index
            int heapIndex = (int)Value;
            Memory.Instance!.Heap.IncrementReferenceCounter(heapIndex);
        }

        private Literal? literal;
        private VarType? ComputeVarTypeLiteral()
        {
            literal = KeyStrings[0]._Literal;
            return literal!._VarType;
        }

        private void ComputeLiteral()
        {
            Value = literal!.Value;
        }

        private ArrayConstruction? arrayConstruction;
        private VarType? ComputeVarTypeArrayConstruction()
        {
            arrayConstruction = KeyStrings[1]._ArrayConstruction;
            return arrayConstruction!._VarType;
        }

        private void ComputeArrayConstruction()
        {
            arrayConstruction!.ArrayLengthExpression.Compute();
            arrayConstruction.ArrayLength = (int)arrayConstruction.ArrayLengthExpression.Value!;
            if (arrayConstruction.ArrayLength < 0)
                throw new ExpressionUnitException(this, "Array length must not be negative!");
            IEnumerable<Variable> variables = Variable.GetBlankVariables(arrayConstruction._VarType.Unarray!, arrayConstruction.ArrayLength);
            int heapIndex = Memory.Instance!.Heap.Allocate(arrayConstruction.ArrayLength, variables);
            Value = heapIndex;
        }

        List<Expression> arrayLiteralExpressions = new();
        private VarType? ComputeVarTypeArrayLiteral()
        {
            string varTypeAsString = KeyStrings[1].Text;
            VarType varType = VarType.GetVarType(varTypeAsString)!;
            if (KeyStrings[2]._Type != KeyString.Type.OpenCurlyBrace)
                throw new ExpressionUnitException(this,
                    "\"{\" expected");
            if (KeyStrings[KeyStrings.Length - 1]._Type != KeyString.Type.CloseCurlyBrace)
                throw new ExpressionUnitException(this,
                    "\"}\" expected");
            KeyString[] keyStringsInsideBraces = new KeyString[KeyStrings.Length - 4];
            Array.Copy(KeyStrings, 3, keyStringsInsideBraces, 0, keyStringsInsideBraces.Length);
            foreach (Expression expression in GetExpressionsBetweenCommas(keyStringsInsideBraces))
            {
                if (expression._VarType != varType.Unarray)
                    if (expression._VarType != null || varType.Unarray!._Storage != VarType.Storage.Reference)
                        throw new ExpressionUnitException(this,
                            $"Cannot have variable of type {expression._VarType} in array of type {varType}");
                arrayLiteralExpressions.Add(expression);
            }
            return varType;
        }

        private IEnumerable<Expression> GetExpressionsBetweenCommas(KeyString[] keyStrings)
        {
            if (keyStrings.Length == 0)
                yield break;
            List<KeyString> currentKeyStrings = new();
            int bracketsOpened = 0;
            foreach (KeyString keyString in keyStrings)
            {
                bool isOpenBracket =
                    keyString._Type == KeyString.Type.OpenBracket ||
                    keyString._Type == KeyString.Type.OpenCurlyBrace ||
                    keyString._Type == KeyString.Type.OpenSquareBrace;
                bool isCloseBracket =
                    keyString._Type == KeyString.Type.CloseBracket ||
                    keyString._Type == KeyString.Type.CloseCurlyBrace ||
                    keyString._Type == KeyString.Type.CloseSquareBrace;
                if (isOpenBracket)
                    bracketsOpened++;
                if (isCloseBracket)
                {
                    bracketsOpened--;
                    if (bracketsOpened == -1)
                        throw new ExpressionUnitException(this, "Closed bracket without opening it");
                }
                if (keyString._Type == KeyString.Type.Comma && bracketsOpened == 0)
                {
                    yield return currentKeyStrings.Count > 0 ?
                        new(currentKeyStrings.ToArray()) :
                        throw new ExpressionUnitException(this, "Unexpected comma");
                    currentKeyStrings = new();
                    continue;
                }
                currentKeyStrings.Add(keyString);
            }
            if (currentKeyStrings.Count > 0)
                yield return new(currentKeyStrings.ToArray());
        }

        private void ComputeArrayLiteral()
        {
            foreach (Expression expression in arrayLiteralExpressions)
                expression.Compute();
            Variable[] variablesToAllocate = new Variable[arrayLiteralExpressions.Count];
            for (int i = 0; i < variablesToAllocate.Length; i++)
            {
                variablesToAllocate[i] = new(_VarType!.Unarray!);
                variablesToAllocate[i].Value = arrayLiteralExpressions[i].Value;
            }
            int heapIndex = Memory.Instance!.Heap.Allocate(variablesToAllocate.Length, variablesToAllocate);
            Value = heapIndex;
        }

        private ArrayElement? arrayElement;
        private VarType? ComputeVarTypeArrayElement()
        {
            arrayElement = KeyStrings[0]._ArrayElement;
            return arrayElement!.Array._VarType!.Unarray;
        }

        private void ComputeArrayElement()
        {
            arrayElement!.IndexExpression.Compute();
            arrayElement.Index = (int)arrayElement.IndexExpression.Value!;
            int index = arrayElement.Index;
            Variable array = arrayElement.Array;
            if (array.Value == null)
                throw new ExpressionUnitException(this, $"Array \"{array.Name}\" has value null");
            int heapIndex = (int)array.Value;
            int length = (int)Memory.Instance!.Heap[heapIndex]!.Value!;
            if (index >= length)
                throw new ExpressionUnitException(this, $"Index {index} out of range of array \"{array.Name}\"");
            Value = Memory.Instance.Heap.GetValue(heapIndex, index);

            if (array._VarType!.Unarray!._Storage == VarType.Storage.Value)
                return;
            if (Value == null)
                return;
            // if it's a reference type with a non-null value then its value is the heap index
            int innerHeapIndex = (int)Value;
            Memory.Instance.Heap.IncrementReferenceCounter(innerHeapIndex);
        }

        private VarType? ComputeVarTypeNull()
        {
            return null;
        }

        private void ComputeNull()
        {
            Value = null;
        }

        private ArrayLength? arrayLength;
        private VarType? ComputeVarTypeArrayLength()
        {
            arrayLength = KeyStrings[0]._ArrayLength;
            return VarType.@int;
        }

        private void ComputeArrayLength()
        {
            Value = arrayLength!.Length;
        }

        private StringLength? stringLength;
        private VarType? ComputeVarTypeStringLength()
        {
            stringLength = KeyStrings[0]._StringLength;
            return VarType.@int;
        }

        private void ComputeStringLength()
        {
            Value = stringLength!.Length;
        }

        private Expression? bracketExpression;
        private VarType? ComputeVarTypeBracket()
        {
            KeyString[] innerKeyStrings = new KeyString[KeyStrings.Length - 2];
            Array.Copy(KeyStrings, 1, innerKeyStrings, 0, innerKeyStrings.Length);
            bracketExpression = new(innerKeyStrings);
            return bracketExpression._VarType;
        }

        private void ComputeBracket()
        {
            bracketExpression!.Compute();
            Value = bracketExpression.Value;
        }

        private StringConstruction? stringConstruction;
        private VarType? ComputeVarTypeStringConstruction()
        {
            KeyString[] charArrayKeyStrings = new KeyString[KeyStrings.Length - 4];
            Array.Copy(KeyStrings, 3, charArrayKeyStrings, 0, charArrayKeyStrings.Length);
            stringConstruction = new(charArrayKeyStrings);
            return VarType.@string;
        }

        private void ComputeStringConstruction()
        {
            stringConstruction!.CharArrayExpression.Compute();
            stringConstruction.CharArrayReference = stringConstruction.CharArrayExpression.Value as int?;
            if (stringConstruction.CharArrayReference == null)
                throw new ExpressionUnitException(this, "Cannot create string from a null character array");
            int charArrayHeapIndex = (int)stringConstruction.CharArrayReference;

            int length = Memory.Instance!.Heap.GetLength(charArrayHeapIndex);
            Variable[] variablesToAllocate = new Variable[length];
            for (int i = 0; i < variablesToAllocate.Length; i++)
            {
                variablesToAllocate[i] = new(VarType.@char);
                variablesToAllocate[i].Value = (char)Memory.Instance.Heap.GetValue(charArrayHeapIndex, i)!;
            }

            // when we inserted the char array into the constructor,
            //      we strictly speaking added a reference to the char array (albeit a temporary one)
            // here we decrement the reference counter to ensure the char array gets garbage collected appropriately
            Memory.Instance!.Heap.DecrementReferenceCounter(charArrayHeapIndex);

            int heapIndex = Memory.Instance.Heap.Allocate(length, variablesToAllocate);
            Value = heapIndex;
        }

        private StringLiteral? stringLiteral;
        private VarType? ComputeVarTypeStringLiteral()
        {
            stringLiteral = KeyStrings[0]._StringLiteral;
            return VarType.@string;
        }

        private void ComputeStringLiteral()
        {
            Variable[] variablesToAllocate = new Variable[stringLiteral!.Value.Length];
            for (int i = 0; i < variablesToAllocate.Length; i++)
            {
                variablesToAllocate[i] = new(VarType.@char);
                variablesToAllocate[i].Value = stringLiteral.Value[i];
            }
            int heapIndex = Memory.Instance!.Heap.Allocate(variablesToAllocate.Length, variablesToAllocate);
            Value = heapIndex;
        }

        private StringElement? stringElement;
        private VarType? ComputeVarTypeStringElement()
        {
            stringElement = KeyStrings[0]._StringElement;
            return VarType.@char;
        }

        private void ComputeStringElement()
        {
            stringElement!.IndexExpression.Compute();
            stringElement.Index = (int)stringElement.IndexExpression.Value!;
            int index = stringElement.Index;
            Variable @string = stringElement.String;
            if (@string.Value == null)
                throw new ExpressionUnitException(this, $"String \"{@string.Name}\" has value null");
            int heapIndex = (int)@string.Value;
            int length = (int)Memory.Instance!.Heap[heapIndex]!.Value!;
            if (index >= length)
                throw new ExpressionUnitException(this, $"Index {index} out of range of string \"{@string.Name}\"");
            Value = Memory.Instance.Heap.GetValue(heapIndex, index);
        }

        private ArrayStringElement? arrayStringElement;
        private VarType? ComputeVarTypeArrayStringElement()
        {
            arrayStringElement = KeyStrings[0]._ArrayStringElement;
            return VarType.@char;
        }

        private void ComputeArrayStringElement()
        {
            arrayStringElement!.ArrayIndexExpression.Compute();
            arrayStringElement!.StringIndexExpression.Compute();
            arrayStringElement.ArrayIndex = (int)arrayStringElement.ArrayIndexExpression.Value!;
            arrayStringElement.StringIndex = (int)arrayStringElement.StringIndexExpression.Value!;
            int arrayIndex = arrayStringElement.ArrayIndex;
            int stringIndex = arrayStringElement.StringIndex;
            Variable array = arrayStringElement.Array;
            if (array.Value == null)
                throw new ExpressionUnitException(this, $"Array \"{array.Name}\" has value null");
            int arrayHeapIndex = (int)array.Value;
            int arrayLength = (int)Memory.Instance!.Heap[arrayHeapIndex]!.Value!;
            if (arrayIndex >= arrayLength)
                throw new ExpressionUnitException(this, $"Index {arrayIndex} out of range of array \"{array.Name}\"");
            object? stringHeapIndexAsObject = Memory.Instance.Heap.GetValue(arrayHeapIndex, arrayIndex);
            if (stringHeapIndexAsObject == null)
                throw new ExpressionUnitException(this, $"String \"{array.Name}[{arrayIndex}]\" has value null");
            int stringHeapIndex = (int)stringHeapIndexAsObject;
            int stringLength = (int)Memory.Instance!.Heap[stringHeapIndex]!.Value!;
            if (stringIndex >= stringLength)
                throw new ExpressionUnitException(this, $"Index {stringIndex} out of range of string \"{array.Name}[{arrayIndex}]\"");
            Value = Memory.Instance.Heap.GetValue(stringHeapIndex, stringIndex);
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
            Bracket,
            StringLiteral,
            StringElement,
            ArrayStringElement,
            StringLength,
            StringConstruction
        }

        public override string ToString()
        {
            string str = string.Empty;
            foreach (KeyString keyString in GetKeyStrings())
                str += keyString.Text + ' ';
            return str;
        }

        public class ExpressionUnitException : InterpreterException
        {
            public ExpressionUnitException(ExpressionUnit? expressionUnit, string? message = null) : base(expressionUnit?.ToString() + " " + message)
            {
                this.expressionUnit = expressionUnit;
            }

            public ExpressionUnit? expressionUnit;
        }
    }
}
