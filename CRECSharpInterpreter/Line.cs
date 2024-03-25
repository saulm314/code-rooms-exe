using System;
using System.Collections.Generic;
using System.Linq;
using CRECSharpInterpreter.Collections.Generic;

namespace CRECSharpInterpreter
{
    public class Line
    {
        public Line(string text)
        {
            Text = text;
            string[] keyStringsStr = KeyString.GetKeyStringsAsStrings(Text);
            KeyStrings = new KeyString[keyStringsStr.Length];
            for (int i = 0; i < KeyStrings.Length; i++)
                KeyStrings[i] = new(keyStringsStr[i]);

            _Type = GetType();
            switch (_Type)
            {
                case Type.Invalid:
                    throw new LineException(this, $"Unrecognised operation in line:\n{Text}");
                case Type.EmptyLine:
                    break;
                case Type.Declaration:
                    VerifyDeclarationValid();
                    DeclareVariable();
                    break;
                case Type.DeclarationInitialisation:
                    VerifyDeclarationValid();
                    _Expression = CreateExpression(); // must be done before declaring variable to ensure no self-references
                    VarToWrite = DeclareVariable();
                    VerifyWriteVariableValid();
                    VarToWrite.Initialised = true;
                    break;
                case Type.WriteVariable:
                    _Expression = CreateExpression();
                    VarToWrite = GetVarToWrite_NoDeclaration();
                    VerifyWriteVariableValid();
                    VarToWrite.Initialised = true;
                    break;
                case Type.WriteArrayElement:
                    _Expression = CreateExpression();
                    ElementToWrite = GetElementToWrite();
                    VerifyWriteArrayElementValid();
                    break;
            }

            if (Memory.Instance!._Mode == Mode.Runtime)
                InitialiseRuntime();
        }

        public string Text { get; init; }

        public KeyString[] KeyStrings { get; init; }

        public Type _Type { get; init; }

        public Variable? VarToWrite { get; init; }

        public ArrayElement? ElementToWrite { get; init; }

        public Expression? _Expression { get; init; }

        public static string[] GetLinesAsStrings(string text)
        {
            text = RemoveMultiLineComments(text);
            text = RemoveSingleLineComments(text);
            List<int> semicolonIndexes = GetSemicolonIndexesFromText(text);
            List<Pair<int, int>> quoteIndexPairs = GetQuoteIndexPairsFromText(text);
            RemoveSemicolonIndexesWithinQuotes(semicolonIndexes, quoteIndexPairs);
            return SplitTextBetweenIndexes(text, semicolonIndexes);
        }

        private static string[] SplitTextBetweenIndexes(string text, List<int> indexes)
        {
            if (indexes.Count == 0)
                return new string[] { text };
            List<string> splits = new();
            string firstSplit = text[..indexes[0]];
            splits.Add(firstSplit);
            for (int i = 0; i < indexes.Count - 1; i++)
            {
                int currentIndex = indexes[i];
                int nextIndex = indexes[i + 1];
                string split = text[(currentIndex + 1)..nextIndex];
                splits.Add(split);
            }
            string finalSplit = text[(indexes[indexes.Count - 1] + 1)..];
            splits.Add(finalSplit);
            return splits.ToArray();
        }

        private static void RemoveSemicolonIndexesWithinQuotes(List<int> semicolonIndexes, List<Pair<int, int>> quoteIndexPairs)
        {
            semicolonIndexes.RemoveAll(index => IsIndexBetweenAnyPairs(index, quoteIndexPairs));
        }

        private static bool IsIndexBetweenAnyPairs(int index, List<Pair<int, int>> pairs)
        {
            foreach (Pair<int, int> pair in pairs)
                if (IsIndexBetweenPairIndexes(index, pair))
                    return true;
            return false;
        }

        private static bool IsIndexBetweenPairIndexes(int index, Pair<int, int> pair)
        {
            return pair.First < index && index < pair.Second;
        }

        public static List<Pair<int, int>> GetQuoteIndexPairsFromText(string text)
        {
            List<Pair<int, int>> pairs = new();
            int index = 0;
            while (index < text.Length)
            {
                int quoteIndex = text.IndexOf('"', index);
                if (quoteIndex == -1)
                    break;
                int nextQuoteIndex = text.IndexOf('"', quoteIndex);
                int nextNewlineIndex = text.IndexOf('\n', quoteIndex);
                if (nextQuoteIndex == -1)
                    throw new LineException(null, "Quote was never closed");
                if (nextNewlineIndex != -1 && nextNewlineIndex <= nextQuoteIndex)
                    throw new LineException(null, "Quote was not closed before newline character");
                Pair<int, int> pair = new(quoteIndex, nextQuoteIndex);
                pairs.Add(pair);
                index = nextQuoteIndex + 1;
            }
            return pairs;
        }

        private static List<int> GetSemicolonIndexesFromText(string text)
        {
            List<int> semicolonIndexes = new();
            int index = 0;
            while (index < text.Length)
            {
                int semicolonIndex = text.IndexOf(';', index);
                if (semicolonIndex == -1)
                    break;
                semicolonIndexes.Add(semicolonIndex);
                index = semicolonIndex + 1;
            }
            return semicolonIndexes;
        }

        private static string RemoveMultiLineComments(string text)
        {
            int commentStartIndex = text.IndexOf("/*");
            while (commentStartIndex != -1)
            {
                int commentEndIndex = text.IndexOf("*/");
                if (commentEndIndex == -1)
                    throw new LineException(null, "Multi-line comment is never closed");
                if (commentEndIndex <= commentStartIndex)
                    throw new LineException(null, "Unexpected \"*/\"");
                text = text.Remove(commentStartIndex, commentEndIndex - commentStartIndex + 2);
                commentStartIndex = text.IndexOf("/*");
            }
            return text;
        }

        // this assumes that multi line comments have already been removed
        private static string RemoveSingleLineComments(string text)
        {
            int commentStartIndex = text.IndexOf("//");
            while (commentStartIndex != -1)
            {
                int nextNewlineIndex = text.IndexOf("\n", commentStartIndex);
                if (nextNewlineIndex == -1)
                {
                    text = text.Remove(commentStartIndex);
                    break;
                }
                text = text.Remove(commentStartIndex, nextNewlineIndex - commentStartIndex);
                commentStartIndex = text.IndexOf("//");
            }
            return text;
        }

        private void InitialiseRuntime()
        {
            string separator = Interpreter.SEPARATOR;
            Console.WriteLine(Text.TrimStart() + '\n');
            switch (_Type)
            {
                case Type.DeclarationInitialisation:
                case Type.WriteVariable:
                    PerformWriteVariable();
                    break;
                case Type.WriteArrayElement:
                    PerformWriteArrayElement();
                    break;
            }
            Console.WriteLine("Stack:");
            Console.WriteLine(separator + "\n");
            foreach (Scope scope in Memory.Instance!.Stack)
            {
                foreach (Variable variable in scope.DeclaredVariables)
                    Console.WriteLine(variable);
                Console.WriteLine(separator);
            }
            Console.WriteLine("\nHeap:\n");
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                    Console.Write(string.Format("{0,10}", Memory.Instance.Heap[10 * i + j]?.ValueAsString ?? "x"));
                Console.Write("\n");
            }
            Console.WriteLine();
            Console.WriteLine(separator + separator + separator);
        }

        private void VerifyDeclarationValid()
        {
            string varName = KeyStrings[1].Text;
            if (Memory.Instance!.IsDeclared(varName))
                throw new LineException(this, $"Variable {varName} has already been declared");
        }

        private Variable DeclareVariable()
        {
            VarType varType = VarType.GetVarType(KeyStrings[0].Text)!;
            string varName = KeyStrings[1].Text;
            Variable declaredVariable = new(varType, varName);
            Memory.Instance!.AddToCurrentScope(declaredVariable);
            return declaredVariable;
        }

        private Expression CreateExpression()
        {
            int expressionOffset = _Type switch
            {
                Type.DeclarationInitialisation => 3,
                Type.WriteVariable => 2,
                Type.WriteArrayElement => 2,
                _ => throw new LineException(this, "internal error")
            };
            KeyString[] expressionKeyStrings = new KeyString[KeyStrings.Length - expressionOffset];
            Array.Copy(KeyStrings, expressionOffset, expressionKeyStrings, 0, expressionKeyStrings.Length);
            return new(expressionKeyStrings);
        }

        private void VerifyWriteVariableValid()
        {
            if (_Expression!._VarType == null)
            {
                if (VarToWrite!._VarType!.DefaultValue == null)
                    return;
                throw new LineException(this,
                    $"Cannot write null to the value type variable \"{VarToWrite.Name}\"");
            }
            if (VarToWrite!._VarType != _Expression._VarType)
                throw new LineException(this,
                    $"Cannot write expression of type {_Expression._VarType} to variable {VarToWrite.Name} of type {VarToWrite._VarType}");
        }

        private void VerifyWriteArrayElementValid()
        {
            if (_Expression!._VarType == null)
            {
                if (ElementToWrite!.Array._VarType!.Unarray!.DefaultValue == null)
                    return;
                throw new LineException(this,
                    $"Cannot write null to a value type element of array \"{ElementToWrite.Array.Name}\"");
            }
            if (ElementToWrite!.Array._VarType!.Unarray != _Expression._VarType)
                throw new LineException(this,
                    $"Cannot write expression of type {_Expression._VarType} to element of array " +
                    $"{ElementToWrite.Array.Name} of type {ElementToWrite.Array._VarType.Unarray}");
        }

        private Variable GetVarToWrite_NoDeclaration()
        {
            Variable? varToWrite = Memory.Instance!.GetVariable(KeyStrings[0].Text);
            if (varToWrite == null)
                throw new LineException(this, $"Variable {KeyStrings[0].Text} hasn't been declared");
            return varToWrite;
        }

        private ArrayElement GetElementToWrite()
        {
            return KeyStrings[0]._ArrayElement!;
        }

        private void PerformWriteVariable()
        {
            _Expression!.Compute();

            bool isReferenceType = VarToWrite!._VarType!._Storage == VarType.Storage.Reference;
            bool isNull = VarToWrite.Value == null;
            if (isReferenceType && !isNull)
            {
                int oldAllocation = (int)VarToWrite.Value!;
                Memory.Instance!.Heap.DecrementReferenceCounter(oldAllocation);
            }

            VarToWrite.Value = _Expression.Value;
        }

        private void PerformWriteArrayElement()
        {
            _Expression!.Compute();
            if (ElementToWrite!.Array.Value == null)
                throw new LineException(this, $"Array \"{ElementToWrite.Array.Name}\" has value null");
            int heapIndex = (int)ElementToWrite.Array.Value;
            ElementToWrite.IndexExpression.Compute();
            ElementToWrite.Index = (int)ElementToWrite.IndexExpression.Value!;
            int index = ElementToWrite.Index;

            bool isReferenceType = ElementToWrite.Array._VarType!.Unarray!._Storage == VarType.Storage.Reference;
            bool isNull = Memory.Instance!.Heap.GetValue(heapIndex, index) == null;
            if (isReferenceType && !isNull)
            {
                int oldAllocation = (int)Memory.Instance!.Heap.GetValue(heapIndex, index)!;
                Memory.Instance.Heap.DecrementReferenceCounter(oldAllocation);
            }

            Memory.Instance.Heap.SetValue(heapIndex, index, _Expression.Value);
        }

        private new Type GetType()
        {
            if (IsEmptyLine)                    return Type.EmptyLine;
            if (IsDeclaration)                  return Type.Declaration;
            if (IsDeclarationInitialisation)    return Type.DeclarationInitialisation;
            if (IsWriteVariable)                return Type.WriteVariable;
            if (IsWriteArrayElement)            return Type.WriteArrayElement;
                                                return Type.Invalid;
        }

        private bool IsEmptyLine { get => _isEmptyLine ??= KeyStrings.Length == 0; }
        private bool? _isEmptyLine;

        private bool IsDeclaration
        {
            get =>
                _isDeclaration ??=
                    KeyStrings.Length == 2 &&
                    KeyStrings[0]._Type == KeyString.Type.Type &&
                    KeyStrings[1]._Type == KeyString.Type.Variable;
        }
        private bool? _isDeclaration;

        private bool IsDeclarationInitialisation
        {
            get =>
                _isDeclarationInitialisation ??=
                    KeyStrings.Length >= 4 &&
                    KeyStrings[0]._Type == KeyString.Type.Type &&
                    KeyStrings[1]._Type == KeyString.Type.Variable &&
                    KeyStrings[2]._Type == KeyString.Type.Equals;
        }
        private bool? _isDeclarationInitialisation;

        private bool IsWriteVariable
        {
            get =>
                _isWriteVariable ??=
                    KeyStrings.Length >= 3 &&
                    KeyStrings[0]._Type == KeyString.Type.Variable &&
                    KeyStrings[1]._Type == KeyString.Type.Equals;
        }
        private bool? _isWriteVariable;

        private bool IsWriteArrayElement
        {
            get =>
                _isWriteArrayElement ??=
                    KeyStrings.Length >= 3 &&
                    KeyStrings[0]._Type == KeyString.Type.ArrayElement &&
                    KeyStrings[1]._Type == KeyString.Type.Equals;
        }
        private bool? _isWriteArrayElement;

        public enum Type
        {
            Invalid,
            EmptyLine,
            Declaration,
            DeclarationInitialisation,
            WriteVariable,
            WriteArrayElement
        }

        public class LineException : InterpreterException
        {
            public LineException(Line? line, string? message = null) : base(message)
            {
                this.line = line;
            }

            public Line? line;
        }
    }
}
