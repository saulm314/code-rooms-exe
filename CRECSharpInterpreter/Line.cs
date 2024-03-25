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
                case Type.WriteStringElement:
                case Type.WriteArrayStringElement:
                    throw new LineException(this, "Cannot write to an element of a string because strings are immutable");
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
            text = RemoveComments(text);

            List<Pair<int, int>> quoteIndexPairs = GetQuoteIndexPairs(text);
            List<Pair<int, int>> bracketIndexPairs = GetBracketIndexPairs(text, quoteIndexPairs);

            List<int> semicolonIndexes = GetRelevantSemicolonIndexes(text, quoteIndexPairs, bracketIndexPairs);
            List<int> closeCurlyBraceIndexes = GetRelevantCloseBraceIndexes(text, bracketIndexPairs);

            List<int> lineEndIndexes = CombineListsAndSort(semicolonIndexes, closeCurlyBraceIndexes);

            string[] lines = SplitTextBetweenIndexesInclusive(text, lineEndIndexes);
            return lines;
        }

        private static string RemoveComments(string text)
        {
            text = RemoveMultiLineComments(text);
            text = RemoveSingleLineComments(text);
            return text;
        }

        private static List<int> GetRelevantSemicolonIndexes(string text, List<Pair<int, int>> quoteIndexPairs, List<Pair<int, int>> bracketIndexPairs)
        {
            List<int> semicolonIndexes = GetSemicolonIndexesFromText(text);
            RemoveIndexesWithinPairs(semicolonIndexes, quoteIndexPairs);
            RemoveIndexesWithinPairs(semicolonIndexes, bracketIndexPairs);
            RemoveIndexesFollowedByElse(text, semicolonIndexes);
            return semicolonIndexes;
        }

        private static List<T> CombineListsAndSort<T>(List<T> list1, List<T> list2)
        {
            List<T> combinedList = new();
            combinedList.AddRange(list1);
            combinedList.AddRange(list2);
            combinedList.Sort();
            return combinedList;
        }

        private static string[] SplitTextBetweenIndexesInclusive(string text, List<int> indexes)
        {
            if (indexes.Count == 0)
                return new string[] { text };
            List<string> splits = new();
            string firstSplit = text[..(indexes[0] + 1)];
            splits.Add(firstSplit);
            for (int i = 0; i < indexes.Count - 1; i++)
            {
                int currentIndex = indexes[i];
                int nextIndex = indexes[i + 1];
                string split = text[(currentIndex + 1)..(nextIndex + 1)];
                splits.Add(split);
            }
            string finalSplit = text[(indexes[indexes.Count - 1] + 1)..];
            splits.Add(finalSplit);
            splits.RemoveAll(str => string.IsNullOrWhiteSpace(str));
            return splits.ToArray();
        }

        private static void RemoveIndexesFollowedByElse(string text, List<int> indexes)
        {
            indexes.RemoveAll(index => IsIndexBeforeElse(text, index));
        }

        private static bool IsIndexBeforeElse(string text, int index)
        {
            int firstNonWhiteSpaceIndexAfterIndex = GetFirstNonWhiteSpaceIndexAfterIndex(text, index);
            if (index >= text.Length - 3)
                return false;
            string firstFourCharacters = text[firstNonWhiteSpaceIndexAfterIndex..(firstNonWhiteSpaceIndexAfterIndex + 4)];
            if (firstFourCharacters == "else")
                return true;
            return false;
        }

        private static int GetFirstNonWhiteSpaceIndexAfterIndex(string text, int index)
        {
            for (int i = index + 1; i < text.Length; i++)
                if (!char.IsWhiteSpace(text[i]))
                    return i;
            return -1;
        }

        public static void RemoveIndexesWithinPairs(List<int> indexes, List<Pair<int, int>> pairs)
        {
            indexes.RemoveAll(index => IsIndexBetweenAnyPairs(index, pairs));
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

        // relevant means close curly braces '}' that denote a line end
        // for example:     if (something) { something; }   the '}' here denotes a line end
        // example:     for (int i = 0; i < length; i++) { something; }     the '}' here denotes a line end
        // example:     if (something) { something; } else { something; }       only the second '}' denotes a line end
        // example:     int[] array = new int[] { 5, 3 };       the '}' does *not* denote a line end
        private static List<int> GetRelevantCloseBraceIndexes(string text, List<Pair<int, int>> bracketIndexPairs)
        {
            List<int> closeCurlyBraceIndexes = new();
            foreach (Pair<int, int> bracketIndexPair in bracketIndexPairs)
                if (text[bracketIndexPair.Second] == '}' && IsCurlyBracePairAfterCloseBracketOrElse(text, bracketIndexPair))
                    closeCurlyBraceIndexes.Add(bracketIndexPair.Second);
            RemoveIndexesFollowedByElse(text, closeCurlyBraceIndexes);
            return closeCurlyBraceIndexes;
        }

        private static bool IsCurlyBracePairAfterCloseBracketOrElse(string text, Pair<int, int> curlyBracePair)
        {
            int lastNonWhiteSpaceIndexBeforeCurlyBraces = GetLastNonWhiteSpaceIndexBeforeIndex(text, curlyBracePair.First);
            if (lastNonWhiteSpaceIndexBeforeCurlyBraces == -1)
                return false;
            if (text[lastNonWhiteSpaceIndexBeforeCurlyBraces] == ')')
                return true;
            if (lastNonWhiteSpaceIndexBeforeCurlyBraces < 3)
                return false;
            string lastFourCharacters = text[(lastNonWhiteSpaceIndexBeforeCurlyBraces - 3)..(lastNonWhiteSpaceIndexBeforeCurlyBraces + 1)];
            if (lastFourCharacters == "else")
                return true;
            return false;
        }

        private static int GetLastNonWhiteSpaceIndexBeforeIndex(string text, int index)
        {
            for (int i = index - 1; i >= 0; i--)
                if (!char.IsWhiteSpace(text[i]))
                    return i;
            return -1;
        }

        // the pairs will consist of any type of open bracket and any type of close bracket
        // e.g. ( ), { }, ( ] will all be valid pairs
        // only top level brackets are considered, e.g. ([])    only the () will be returned
        public static List<Pair<int, int>> GetBracketIndexPairs(string text, List<Pair<int, int>> quoteIndexPairs)
        {
            List<Pair<int, int>> pairs = new();
            Pair<int, int> currentPair = new(default, default);
            int bracketsOpened = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (IsIndexBetweenAnyPairs(i, quoteIndexPairs))
                    continue;
                bool isOpenBracket =
                    text[i] == '(' ||
                    text[i] == '[' ||
                    text[i] == '{';
                bool isCloseBracket =
                    text[i] == ')' ||
                    text[i] == ']' ||
                    text[i] == '}';
                if (isOpenBracket)
                {
                    if (bracketsOpened == 0)
                        currentPair.First = i;
                    bracketsOpened++;
                    continue;
                }
                if (isCloseBracket)
                {
                    if (bracketsOpened <= 0)
                        throw new LineException(null, "Closed bracket without opening it");
                    if (bracketsOpened == 1)
                    {
                        currentPair.Second = i;
                        pairs.Add(currentPair);
                        currentPair = new(default, default);
                    }
                    bracketsOpened--;
                    continue;
                }
            }
            if (bracketsOpened != 0)
                throw new LineException(null, "Opened bracket without closing it");
            return pairs;
        }

        public static List<Pair<int, int>> GetQuoteIndexPairs(string text)
        {
            List<Pair<int, int>> pairs = new();
            Pair<int, int> currentPair = new(default, default);
            QuoteState quoteState = QuoteState.NoQuote;
            for (int i = 0; i < text.Length; i++)
            {
                switch (quoteState)
                {
                    case QuoteState.DoubleQuote:
                        if (text[i] == '"')
                        {
                            currentPair.Second = i;
                            pairs.Add(currentPair);
                            currentPair = new(default, default);
                            quoteState = QuoteState.NoQuote;
                        }
                        break;
                    case QuoteState.SingleQuote:
                        if (text[i] == '\'')
                        {
                            currentPair.Second = i;
                            pairs.Add(currentPair);
                            currentPair = new(default, default);
                            quoteState = QuoteState.NoQuote;
                        }
                        break;
                    case QuoteState.NoQuote:
                        if (text[i] == '"')
                        {
                            currentPair.First = i;
                            quoteState = QuoteState.DoubleQuote;
                            break;
                        }
                        if (text[i] == '\'')
                        {
                            currentPair.First = i;
                            quoteState = QuoteState.SingleQuote;
                            break;
                        }
                        break;
                }
            }
            if (quoteState != QuoteState.NoQuote)
                throw new LineException(null, "Quote was never closed");
            return pairs;
        }

        private enum QuoteState
        {
            NoQuote,
            SingleQuote,
            DoubleQuote
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
            KeyString[] expressionKeyStrings = new KeyString[KeyStrings.Length - expressionOffset - 1];
            // not copying the final semicolon
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
            bool isReferenceType = VarToWrite!._VarType!._Storage == VarType.Storage.Reference;
            bool isNull = VarToWrite.Value == null;
            if (isReferenceType && !isNull)
            {
                int oldAllocation = (int)VarToWrite.Value!;
                Memory.Instance!.Heap.DecrementReferenceCounter(oldAllocation);
            }

            _Expression!.Compute();

            VarToWrite.Value = _Expression.Value;
        }

        private void PerformWriteArrayElement()
        {
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

            _Expression!.Compute();

            Memory.Instance.Heap.SetValue(heapIndex, index, _Expression.Value);
        }

        private new Type GetType()
        {
            if (IsEmptyLine)                    return Type.EmptyLine;
            if (IsDeclaration)                  return Type.Declaration;
            if (IsDeclarationInitialisation)    return Type.DeclarationInitialisation;
            if (IsWriteVariable)                return Type.WriteVariable;
            if (IsWriteArrayElement)            return Type.WriteArrayElement;
            if (IsWriteStringElement)           return Type.WriteStringElement;
            if (IsWriteArrayStringElement)      return Type.WriteArrayStringElement;
                                                return Type.Invalid;
        }

        private bool IsEmptyLine
        {
            get =>
                _isEmptyLine ??=
                    KeyStrings.Length == 1 &&
                    KeyStrings[0]._Type == KeyString.Type.Semicolon;
        }
        private bool? _isEmptyLine;

        private bool IsDeclaration
        {
            get =>
                _isDeclaration ??=
                    KeyStrings.Length == 3 &&
                    KeyStrings[0]._Type == KeyString.Type.Type &&
                    KeyStrings[1]._Type == KeyString.Type.Variable &&
                    KeyStrings[2]._Type == KeyString.Type.Semicolon;
        }
        private bool? _isDeclaration;

        private bool IsDeclarationInitialisation
        {
            get =>
                _isDeclarationInitialisation ??=
                    KeyStrings.Length >= 5 &&
                    KeyStrings[0]._Type == KeyString.Type.Type &&
                    KeyStrings[1]._Type == KeyString.Type.Variable &&
                    KeyStrings[2]._Type == KeyString.Type.Equals &&
                    KeyStrings[KeyStrings.Length - 1]._Type == KeyString.Type.Semicolon;
        }
        private bool? _isDeclarationInitialisation;

        private bool IsWriteVariable
        {
            get =>
                _isWriteVariable ??=
                    KeyStrings.Length >= 4 &&
                    KeyStrings[0]._Type == KeyString.Type.Variable &&
                    KeyStrings[1]._Type == KeyString.Type.Equals &&
                    KeyStrings[KeyStrings.Length - 1]._Type == KeyString.Type.Semicolon;
        }
        private bool? _isWriteVariable;

        private bool IsWriteArrayElement
        {
            get =>
                _isWriteArrayElement ??=
                    KeyStrings.Length >= 4 &&
                    KeyStrings[0]._Type == KeyString.Type.ArrayElement &&
                    KeyStrings[1]._Type == KeyString.Type.Equals &&
                    KeyStrings[KeyStrings.Length - 1]._Type == KeyString.Type.Semicolon;
        }
        private bool? _isWriteArrayElement;

        private bool IsWriteStringElement
        {
            get =>
                _isWriteStringElement ??=
                    KeyStrings.Length >= 4 &&
                    KeyStrings[0]._Type == KeyString.Type.StringElement &&
                    KeyStrings[1]._Type == KeyString.Type.Equals &&
                    KeyStrings[KeyStrings.Length - 1]._Type == KeyString.Type.Semicolon;
        }
        private bool? _isWriteStringElement;

        private bool IsWriteArrayStringElement
        {
            get =>
                _isWriteArrayStringElement ??=
                    KeyStrings.Length >= 4 &&
                    KeyStrings[0]._Type == KeyString.Type.ArrayStringElement &&
                    KeyStrings[1]._Type == KeyString.Type.Equals &&
                    KeyStrings[KeyStrings.Length - 1]._Type == KeyString.Type.Semicolon;
        }
        private bool? _isWriteArrayStringElement;

        public enum Type
        {
            Invalid,
            EmptyLine,
            Declaration,
            DeclarationInitialisation,
            WriteVariable,
            WriteArrayElement,
            WriteStringElement,
            WriteArrayStringElement
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
