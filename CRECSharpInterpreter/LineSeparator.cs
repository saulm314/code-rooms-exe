﻿using CRECSharpInterpreter.Collections.Generic;
using System.Collections.Generic;
using System;

namespace CRECSharpInterpreter
{
    public static class LineSeparator
    {
        public static string[] GetLinesAsStrings(string text, ushort startLineNumber, out ushort[] lineNumbers)
        {
            text = RemoveComments(text);

            List<Pair<int, int>> quoteIndexPairs = GetQuoteIndexPairs(text);
            List<Pair<int, int>> bracketIndexPairs = GetBracketIndexPairs(text, quoteIndexPairs);

            List<int> semicolonIndexes = GetRelevantSemicolonIndexes(text, quoteIndexPairs, bracketIndexPairs);
            List<int> closeCurlyBraceIndexes = GetRelevantCloseBraceIndexes(text, bracketIndexPairs);

            List<int> lineEndIndexes = CombineListsAndSort(semicolonIndexes, closeCurlyBraceIndexes);

            string[] lines = SplitTextBetweenIndexesInclusive(text, lineEndIndexes);

            GetLineNumbers(lines, startLineNumber, out lineNumbers);

            return lines;
        }

        private static void GetLineNumbers(string[] lines, ushort startLineNumber, out ushort[] lineNumbers)
        {
            lineNumbers = new ushort[lines.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                ushort[] _lineNumbers = LineNumberUtils.GetLineNumbers(lines[i]);
                if (_lineNumbers.Length > 0)
                {
                    lineNumbers[i] = _lineNumbers[0];
                    continue;
                }
                if (i > 0)
                {
                    lineNumbers[i] = lineNumbers[i - 1];
                    continue;
                }
                lineNumbers[i] = startLineNumber;
            }
        }

        public static string GetForInitialiserAsString(string baseLine)
        {
            List<Pair<int, int>> quoteIndexPairs = GetQuoteIndexPairs(baseLine);
            List<Pair<int, int>> bracketIndexPairs = GetBracketIndexPairs(baseLine, quoteIndexPairs);
            int semicolonIndex = baseLine.IndexOf(';');
            string initialiser = baseLine[(bracketIndexPairs[0].First + 1)..(semicolonIndex + 1)];
            return initialiser;
        }

        public static string GetForIteratorAsString(string baseLine)
        {
            List<Pair<int, int>> quoteIndexPairs = GetQuoteIndexPairs(baseLine);
            List<Pair<int, int>> bracketIndexPairs = GetBracketIndexPairs(baseLine, quoteIndexPairs);
            int semicolonIndex = baseLine.IndexOf(';');
            int secondSemicolonIndex = baseLine.IndexOf(';', semicolonIndex + 1);
            string iterator = baseLine[(secondSemicolonIndex + 1)..bracketIndexPairs[0].Second] + ";";
            return iterator;
        }

        public static string[] GetSubLinesAsStringsIfSingleLine(string baseLine, out string header, ushort startLineNumber, out ushort[] lineNumbers)
        {
            List<Pair<int, int>> quoteIndexPairs = GetQuoteIndexPairs(baseLine);
            List<Pair<int, int>> bracketIndexPairs = GetBracketIndexPairs(baseLine, quoteIndexPairs);
            header = baseLine[..(bracketIndexPairs[0].Second + 1)];
            string subText = baseLine[(bracketIndexPairs[0].Second + 1)..];
            string[] subLines = GetLinesAsStrings(subText, startLineNumber, out lineNumbers);
            return subLines;
        }

        public static string[] GetSubLinesAsStringsElseSingleLine(string baseLine, out string header, ushort startLineNumber, out ushort[] lineNumbers)
        {
            header = "else";
            int indexAfterElse = baseLine.IndexOf("else") + 4;
            string subText = baseLine[indexAfterElse..];
            string[] subLines = GetLinesAsStrings(subText, startLineNumber, out lineNumbers);
            return subLines;
        }

        public static string[] GetSubLinesAsStringsIfMultiLine(string baseLine, out string header, ushort startLineNumber, out ushort[] lineNumbers)
        {
            baseLine = TrimEnd(baseLine);
            List<Pair<int, int>> quoteIndexPairs = GetQuoteIndexPairs(baseLine);
            List<Pair<int, int>> bracketIndexPairs = GetBracketIndexPairs(baseLine, quoteIndexPairs);
            if (bracketIndexPairs.Count < 2)
                throw new InterpreterException("A bracket pair and curly brace pair expected");
            header = baseLine[..(bracketIndexPairs[0].Second + 1)];
            string subText = baseLine[(bracketIndexPairs[1].First + 1)..(baseLine.Length - 1)];
            string[] subLines = GetLinesAsStrings(subText, startLineNumber, out lineNumbers);
            return subLines;
        }

        public static string[] GetSubLinesAsStringsElseMultiLine(string baseLine, out string header, ushort startLineNumber, out ushort[] lineNumbers)
        {
            baseLine = TrimEnd(baseLine);
            header = "else";
            List<Pair<int, int>> quoteIndexPairs = GetQuoteIndexPairs(baseLine);
            List<Pair<int, int>> bracketIndexPairs = GetBracketIndexPairs(baseLine, quoteIndexPairs);
            if (bracketIndexPairs.Count < 1)
                throw new InterpreterException("A curly brace pair expected");
            string subText = baseLine[(bracketIndexPairs[0].First + 1)..(baseLine.Length - 1)];
            string[] subLines = GetLinesAsStrings(subText, startLineNumber, out lineNumbers);
            return subLines;
        }

        public static string[] GetSubLinesAsStringsIfElseIf(string baseLine, out string header, ushort startLineNumber, out ushort[] lineNumbers)
        {
            int elseIndex = baseLine.IndexOf("else");
            string baseLineBeforeElse = baseLine[..elseIndex];
            List<Pair<int, int>> quoteIndexPairs = GetQuoteIndexPairs(baseLineBeforeElse);
            List<Pair<int, int>> bracketIndexPairs = GetBracketIndexPairs(baseLineBeforeElse, quoteIndexPairs);
            int firstNonWhiteSpaceIndexAfterCloseBracket = GetFirstNonWhiteSpaceIndexAfterIndex(baseLineBeforeElse, bracketIndexPairs[0].Second);
            if (firstNonWhiteSpaceIndexAfterCloseBracket == -1)
                throw new InterpreterException("Expecting statement after condition");
            if (baseLineBeforeElse[firstNonWhiteSpaceIndexAfterCloseBracket] == '{')
                return GetSubLinesAsStringsIfMultiLine(baseLineBeforeElse, out header, startLineNumber, out lineNumbers);
            return GetSubLinesAsStringsIfSingleLine(baseLineBeforeElse, out header, startLineNumber, out lineNumbers);
        }

        public static string[] GetSubLinesAsStringsIfElseElse(string baseLine, out string header, ushort startLineNumber, out ushort[] lineNumbers)
        {
            int elseIndex = baseLine.IndexOf("else");
            string baseLineFromElse = baseLine[elseIndex..];
            int lastElseIndex = 3;
            int firstNonWhiteSpaceIndexAfterElse = GetFirstNonWhiteSpaceIndexAfterIndex(baseLineFromElse, lastElseIndex);
            if (firstNonWhiteSpaceIndexAfterElse == -1)
                throw new InterpreterException("Expecting statement after else keyword");
            if (baseLineFromElse[firstNonWhiteSpaceIndexAfterElse] == '{')
                return GetSubLinesAsStringsElseMultiLine(baseLineFromElse, out header, startLineNumber, out lineNumbers);
            return GetSubLinesAsStringsElseSingleLine(baseLineFromElse, out header, startLineNumber, out lineNumbers);
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
                return IsWhiteSpaceOrSeparator(text) ?
                    Array.Empty<string>() :
                    new string[] { text };
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
            splits.RemoveAll(str => IsWhiteSpaceOrSeparator(str));
            return splits.ToArray();
        }

        private static bool IsWhiteSpaceOrSeparator(string text)
        {
            bool _is =
                string.IsNullOrWhiteSpace(text) ||
                Trim(text).Length == 0;
            return _is;
        }

        private static void RemoveIndexesFollowedByElse(string text, List<int> indexes)
        {
            indexes.RemoveAll(index => IsIndexBeforeElse(text, index));
        }

        private static bool IsIndexBeforeElse(string text, int index)
        {
            int firstNonWhiteSpaceIndexAfterIndex = GetFirstNonWhiteSpaceIndexAfterIndex(text, index);
            if (firstNonWhiteSpaceIndexAfterIndex == -1)
                return false;
            if (index >= text.Length - 3)
                return false;
            string firstFourCharacters = text[firstNonWhiteSpaceIndexAfterIndex..(firstNonWhiteSpaceIndexAfterIndex + 4)];
            if (firstFourCharacters == "else")
                return true;
            return false;
        }
        
        // we consider the separator and the 4 characters after to be whitespace
        private static int GetFirstNonWhiteSpaceIndexAfterIndex(string text, int index)
        {
            int i = index + 1;
            while (i < text.Length)
            {
                if (text[i] == LineNumberUtils.SEPARATOR)
                {
                    i += LineNumberUtils.SEPARATOR_LENGTH;
                    continue;
                }
                if (!char.IsWhiteSpace(text[i]))
                    return i;
                i++;
            }
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

        // we take the separator and associated characters to be whitespace
        private static int GetLastNonWhiteSpaceIndexBeforeIndex(string text, int index)
        {
            int i = index - 1;
            while (i >= 0)
            {
                if (char.IsWhiteSpace(text[i]))
                {
                    i--;
                    continue;
                }
                if (i < LineNumberUtils.SEPARATOR_LENGTH - 2)
                    return i;
                if (text[i - LineNumberUtils.SEPARATOR_LENGTH + 1] != LineNumberUtils.SEPARATOR)
                    return i;
                i -= LineNumberUtils.SEPARATOR_LENGTH;
            }
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
                        throw new InterpreterException("Closed bracket without opening it");
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
                throw new InterpreterException("Opened bracket without closing it");
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
                throw new InterpreterException("Quote was never closed");
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
                    throw new InterpreterException("Multi-line comment is never closed");
                if (commentEndIndex <= commentStartIndex)
                    throw new InterpreterException("Unexpected \"*/\"");
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

        public static string TrimStart(string text)
        {
            text = text.TrimStart();
            if (text.Length == 0)
                return text;
            if (text[0] != LineNumberUtils.SEPARATOR)
                return text;
            text = text.Remove(0, LineNumberUtils.SEPARATOR_LENGTH);
            return TrimStart(text);
        }

        public static string TrimEnd(string text)
        {
            text = text.TrimEnd();
            if (text.Length < LineNumberUtils.SEPARATOR_LENGTH)
                return text;
            if (text[text.Length - LineNumberUtils.SEPARATOR_LENGTH] != LineNumberUtils.SEPARATOR)
                return text;
            text = text.Remove(text.Length - LineNumberUtils.SEPARATOR_LENGTH, LineNumberUtils.SEPARATOR_LENGTH);
            return TrimEnd(text);
        }

        public static string Trim(string text)
        {
            text = TrimStart(text);
            text = TrimEnd(text);
            return text;
        }
    }
}