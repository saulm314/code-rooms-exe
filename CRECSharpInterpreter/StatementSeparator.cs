using CRECSharpInterpreter.Collections.Generic;
using System.Collections.Generic;
using System;

namespace CRECSharpInterpreter
{
    public static class StatementSeparator
    {
        public static string[] GetStatementsAsStrings(string text, ushort startLineNumber, out LineNumberInfo[] lineNumberInfos,
                                                        out SuperStatement[] outputSuperStatements)
        {
            text = RemoveComments(text);

            List<Pair<int, int>> quoteIndexPairs = GetQuoteIndexPairs(text);
            List<Pair<int, int>> bracketIndexPairs = GetBracketIndexPairs(text, quoteIndexPairs);
            List<Pair<int, int>> superStatementIndexPairs = GetSuperStatementIndexPairs(text, bracketIndexPairs, out List<SuperStatement> superStatements);

            List<int> semicolonIndexes = GetRelevantSemicolonIndexes(text, quoteIndexPairs, bracketIndexPairs, superStatementIndexPairs);
            List<int> superStatementEndIndexes = GetSuperStatementEndIndexes(superStatementIndexPairs);
            
            List<int> statementEndIndexes = CombineListsAndSort(semicolonIndexes, superStatementEndIndexes);

            string[] statements = SplitTextBetweenIndexesInclusive(text, statementEndIndexes);

            GetLineNumbers(statements, startLineNumber, out lineNumberInfos);
            outputSuperStatements = GetOutputSuperStatements(statementEndIndexes, superStatementEndIndexes, superStatements);

            return statements;
        }

        private static SuperStatement[] GetOutputSuperStatements(List<int> statementEndIndexes, List<int> superStatementEndIndexes,
                                                                    List<SuperStatement> superStatements)
        {
            SuperStatement[] output = new SuperStatement[statementEndIndexes.Count];
            int superStatementsAdded = 0;
            for (int i = 0; i < statementEndIndexes.Count; i++)
            {
                int endIndex = statementEndIndexes[i];
                if (superStatementEndIndexes.Contains(endIndex))
                    output[i] = superStatements[superStatementsAdded++];
            }
            return output;
        }

        private static void GetLineNumbers(string[] statements, ushort startLineNumber, out LineNumberInfo[] lineNumberInfos)
        {
            lineNumberInfos = new LineNumberInfo[statements.Length];
            for (int i = 0; i < statements.Length; i++)
            {
                ushort[] _lineNumbers = LineNumberUtils.GetLineNumbers(statements[i], out ushort actualLineNumber);
                if (_lineNumbers.Length > 0)
                {
                    lineNumberInfos[i] = new(_lineNumbers[0], actualLineNumber, _lineNumbers[^1]);
                    continue;
                }
                if (i > 0)
                {
                    ushort previousLineNumber = lineNumberInfos[i - 1].EndLineNumber;
                    lineNumberInfos[i] = new(previousLineNumber, previousLineNumber, previousLineNumber);
                    continue;
                }
                lineNumberInfos[i] = new(startLineNumber, startLineNumber, startLineNumber);
            }
        }

        public static string GetForInitialiserAsString(string baseStatement)
        {
            List<Pair<int, int>> quoteIndexPairs = GetQuoteIndexPairs(baseStatement);
            List<Pair<int, int>> bracketIndexPairs = GetBracketIndexPairs(baseStatement, quoteIndexPairs);
            int semicolonIndex = baseStatement.IndexOf(';');
            string initialiser = baseStatement[(bracketIndexPairs[0].First + 1)..(semicolonIndex + 1)];
            return initialiser;
        }

        public static string GetForIteratorAsString(string baseStatement)
        {
            List<Pair<int, int>> quoteIndexPairs = GetQuoteIndexPairs(baseStatement);
            List<Pair<int, int>> bracketIndexPairs = GetBracketIndexPairs(baseStatement, quoteIndexPairs);
            int semicolonIndex = baseStatement.IndexOf(';');
            int secondSemicolonIndex = baseStatement.IndexOf(';', semicolonIndex + 1);
            string iterator = baseStatement[(secondSemicolonIndex + 1)..bracketIndexPairs[0].Second] + ";";
            return iterator;
        }

        public static string[] GetSubStatementsAsStringsIfSingleStatement(string baseStatement, out string header, ushort startLineNumber,
                                                                            out LineNumberInfo[] lineNumberInfos, out SuperStatement[] superStatements)
        {
            List<Pair<int, int>> quoteIndexPairs = GetQuoteIndexPairs(baseStatement);
            List<Pair<int, int>> bracketIndexPairs = GetBracketIndexPairs(baseStatement, quoteIndexPairs);
            header = baseStatement[..(bracketIndexPairs[0].Second + 1)];
            string subText = baseStatement[(bracketIndexPairs[0].Second + 1)..];
            string[] subStatements = GetStatementsAsStrings(subText, startLineNumber, out lineNumberInfos, out superStatements);
            return subStatements;
        }

        public static string[] GetSubStatementsAsStringsElseSingleStatement(string baseStatement, out string header, ushort startLineNumber,
                                                                            out LineNumberInfo[] lineNumberInfos, out SuperStatement[] superStatements)
        {
            header = "else";
            int indexAfterElse = baseStatement.IndexOf("else") + 4;
            string subText = baseStatement[indexAfterElse..];
            string[] subStatements = GetStatementsAsStrings(subText, startLineNumber, out lineNumberInfos, out superStatements);
            return subStatements;
        }

        public static string[] GetSubStatementsAsStringsIfMultiStatement(string baseStatement, out string header, ushort startLineNumber,
                                                                            out LineNumberInfo[] lineNumberInfos, out SuperStatement[] superStatements)
        {
            baseStatement = LineNumberUtils.TrimEnd(baseStatement);
            List<Pair<int, int>> quoteIndexPairs = GetQuoteIndexPairs(baseStatement);
            List<Pair<int, int>> bracketIndexPairs = GetBracketIndexPairs(baseStatement, quoteIndexPairs);
            if (bracketIndexPairs.Count < 2)
                throw new InterpreterException("A bracket pair and curly brace pair expected");
            header = baseStatement[..(bracketIndexPairs[0].Second + 1)];
            string subText = baseStatement[(bracketIndexPairs[1].First + 1)..(baseStatement.Length - 1)];
            string[] subStatements = GetStatementsAsStrings(subText, startLineNumber, out lineNumberInfos, out superStatements);
            return subStatements;
        }

        public static string[] GetSubStatementsAsStringsElseMultiStatement(string baseStatement, out string header, ushort startLineNumber,
                                                                            out LineNumberInfo[] lineNumberInfos, out SuperStatement[] superStatements)
        {
            baseStatement = LineNumberUtils.TrimEnd(baseStatement);
            header = "else";
            List<Pair<int, int>> quoteIndexPairs = GetQuoteIndexPairs(baseStatement);
            List<Pair<int, int>> bracketIndexPairs = GetBracketIndexPairs(baseStatement, quoteIndexPairs);
            if (bracketIndexPairs.Count < 1)
                throw new InterpreterException("A curly brace pair expected");
            string subText = baseStatement[(bracketIndexPairs[0].First + 1)..(baseStatement.Length - 1)];
            string[] subStatements = GetStatementsAsStrings(subText, startLineNumber, out lineNumberInfos, out superStatements);
            return subStatements;
        }

        public static string[] GetSubStatementsAsStringsIfElseIf(string baseStatement, out string header, ushort startLineNumber,
                                                                    out LineNumberInfo[] lineNumberInfos, out SuperStatement[] superStatements)
        {
            Console.WriteLine(baseStatement);
            int elseIndex = baseStatement.IndexOf("else");
            string baseStatementBeforeElse = baseStatement[..elseIndex];
            List<Pair<int, int>> quoteIndexPairs = GetQuoteIndexPairs(baseStatementBeforeElse);
            List<Pair<int, int>> bracketIndexPairs = GetBracketIndexPairs(baseStatementBeforeElse, quoteIndexPairs);
            int firstNonWhiteSpaceIndexAfterCloseBracket = GetFirstNonWhiteSpaceIndexAfterIndex(baseStatementBeforeElse, bracketIndexPairs[0].Second);
            if (firstNonWhiteSpaceIndexAfterCloseBracket == -1)
                throw new InterpreterException("Expecting statement after condition");
            if (baseStatementBeforeElse[firstNonWhiteSpaceIndexAfterCloseBracket] == '{')
                return GetSubStatementsAsStringsIfMultiStatement(baseStatementBeforeElse, out header, startLineNumber, out lineNumberInfos, out superStatements);
            return GetSubStatementsAsStringsIfSingleStatement(baseStatementBeforeElse, out header, startLineNumber, out lineNumberInfos, out superStatements);
        }

        public static string[] GetSubStatementsAsStringsIfElseElse(string baseStatement, out string header, ushort startLineNumber,
                                                                    out LineNumberInfo[] lineNumberInfos, out SuperStatement[] superStatements)
        {
            int elseIndex = baseStatement.IndexOf("else");
            string baseStatementFromElse = baseStatement[elseIndex..];
            int lastElseIndex = 3;
            int firstNonWhiteSpaceIndexAfterElse = GetFirstNonWhiteSpaceIndexAfterIndex(baseStatementFromElse, lastElseIndex);
            if (firstNonWhiteSpaceIndexAfterElse == -1)
                throw new InterpreterException("Expecting statement after else keyword");
            if (baseStatementFromElse[firstNonWhiteSpaceIndexAfterElse] == '{')
                return GetSubStatementsAsStringsElseMultiStatement(baseStatementFromElse, out header, startLineNumber, out lineNumberInfos, out superStatements);
            return GetSubStatementsAsStringsElseSingleStatement(baseStatementFromElse, out header, startLineNumber, out lineNumberInfos, out superStatements);
        }

        private static string RemoveComments(string text)
        {
            text = RemoveMultiLineComments(text);
            text = RemoveSingleLineComments(text);
            return text;
        }

        private static List<int> GetRelevantSemicolonIndexes(string text, List<Pair<int, int>> quoteIndexPairs, List<Pair<int, int>> bracketIndexPairs,
                                                                List<Pair<int, int>> superStatementIndexPairs)
        {
            List<int> semicolonIndexes = GetSemicolonIndexesFromText(text);
            RemoveIndexesWithinPairs(semicolonIndexes, quoteIndexPairs);
            RemoveIndexesWithinPairs(semicolonIndexes, bracketIndexPairs);
            RemoveIndexesWithinPairs(semicolonIndexes, superStatementIndexPairs);
            return semicolonIndexes;
        }

        private static List<int> GetSuperStatementEndIndexes(List<Pair<int, int>> superStatementIndexPairs)
        {
            List<int> superStatementEndIndexes = new();
            for (int i = 0; i < superStatementIndexPairs.Count; i++)
                superStatementEndIndexes.Add(superStatementIndexPairs[i].Second);
            return superStatementEndIndexes;
        }

        private static List<T> CombineListsAndSort<T>(List<T> list1, List<T> list2)
        {
            HashSet<T> combinedSet = new();
            foreach (T item in list1)
                combinedSet.Add(item);
            foreach (T item in list2)
                combinedSet.Add(item);
            List<T> combinedList = new();
            combinedList.AddRange(combinedSet);
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
                LineNumberUtils.Trim(text).Length == 0;
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
        public static int GetFirstNonWhiteSpaceIndexAfterIndex(string text, int index)
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

        // relevant means close curly braces '}' that denote a statement end
        // for example:     if (something) { something; }   the '}' here denotes a statement end
        // example:     for (int i = 0; i < length; i++) { something; }     the '}' here denotes a statement end
        // example:     if (something) { something; } else { something; }       only the second '}' denotes a statement end
        // example:     int[] array = new int[] { 5, 3 };       the '}' does *not* denote a statement end
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
        public static int GetLastNonWhiteSpaceIndexBeforeIndex(string text, int index)
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

        private static List<Pair<int, int>> GetSuperStatementIndexPairs(string text, List<Pair<int, int>> bracketIndexPairs,
                                                                        out List<SuperStatement> superStatements)
        {
            superStatements = new();
            List<Pair<int, int>> pairs = new();
            Pair<int, int> currentPair = new(default, default);
            int i = 0;
            while (i < text.Length)
            {
                SuperStatement superStatement = GetOpenSuperStatement(text, i);
                if (superStatement == SuperStatement.None)
                {
                    i++;
                    continue;
                }
                currentPair.First = i;
                int headerEnd = GetHeaderEnd(i, superStatement, bracketIndexPairs);
                int superStatementEnd = GetSuperStatementEnd(text, superStatement, headerEnd, bracketIndexPairs, out SuperStatement finalSuperStatement);
                superStatements.Add(finalSuperStatement);
                currentPair.Second = superStatementEnd;
                pairs.Add(currentPair);
                currentPair = new(default, default);
                i = superStatementEnd + 1;
            }
            return pairs;
        }

        private static int GetHeaderEnd(int headerStart, SuperStatement superStatement, List<Pair<int, int>> bracketIndexPairs)
        {
            switch (superStatement)
            {
                case SuperStatement.Else:
                    return headerStart + 3;
                case SuperStatement.IfUnknown:
                case SuperStatement.WhileUnknown:
                case SuperStatement.ForUnknown:
                    foreach (Pair<int, int> bracketIndexPair in bracketIndexPairs)
                        if (bracketIndexPair.First > headerStart)
                            return bracketIndexPair.Second;
                    throw new InterpreterException("Did not open/close bracket");
                default:
                    throw new InterpreterException("Internal error");
            }
        }

        private static int GetSuperStatementEnd(string text, SuperStatement superStatement, int headerEnd, List<Pair<int, int>> bracketIndexPairs,
                                                out SuperStatement finalSuperStatement)
        {
            switch (superStatement)
            {
                case SuperStatement.IfUnknown:
                case SuperStatement.Else:
                case SuperStatement.WhileUnknown:
                case SuperStatement.ForUnknown:
                    break;
                default:
                    throw new InterpreterException("Internal error");
            }
            int firstNonWhiteSpaceIndexAfterHeader = GetFirstNonWhiteSpaceIndexAfterIndex(text, headerEnd);
            if (firstNonWhiteSpaceIndexAfterHeader == -1)
                throw new InterpreterException("Substatement after if/else/while/for expected");
            int superStatementEnd;
            if (text[firstNonWhiteSpaceIndexAfterHeader] == '{')
            {
                superStatementEnd = GetMultiSuperStatementEnd(firstNonWhiteSpaceIndexAfterHeader, bracketIndexPairs);
                finalSuperStatement = superStatement switch
                {
                    SuperStatement.IfUnknown => SuperStatement.IfMulti,
                    SuperStatement.WhileUnknown => SuperStatement.WhileMulti,
                    SuperStatement.ForUnknown => SuperStatement.ForMulti,
                    SuperStatement.Else => SuperStatement.Else,
                    _ => throw new InterpreterException("Internal error")
                };
            }
            else
            {
                superStatementEnd = GetSingleSuperStatementEnd(text, headerEnd);
                finalSuperStatement = superStatement switch
                {
                    SuperStatement.IfUnknown => SuperStatement.IfSingle,
                    SuperStatement.WhileUnknown => SuperStatement.WhileSingle,
                    SuperStatement.ForUnknown => SuperStatement.ForSingle,
                    SuperStatement.Else => SuperStatement.Else,
                    _ => throw new InterpreterException("Internal error")
                };
            }
            if (superStatement != SuperStatement.IfUnknown)
                return superStatementEnd;
            int firstNonWhiteSpaceIndexAfterSuperStatement = GetFirstNonWhiteSpaceIndexAfterIndex(text, superStatementEnd);
            if (firstNonWhiteSpaceIndexAfterSuperStatement == -1)
                return superStatementEnd;
            SuperStatement nextSuperStatement = GetOpenSuperStatement(text, firstNonWhiteSpaceIndexAfterSuperStatement);
            if (nextSuperStatement != SuperStatement.Else)
                return superStatementEnd;
            finalSuperStatement = SuperStatement.IfElse;
            int nextHeaderEnd = GetHeaderEnd(firstNonWhiteSpaceIndexAfterSuperStatement, SuperStatement.Else, bracketIndexPairs);
            return GetSuperStatementEnd(text, SuperStatement.Else, nextHeaderEnd, bracketIndexPairs, out _);
        }

        private static int GetMultiSuperStatementEnd(int curlyBraceStart, List<Pair<int, int>> bracketIndexPairs)
        {
            foreach (Pair<int, int> bracketIndexPair in bracketIndexPairs)
                if (bracketIndexPair.First == curlyBraceStart)
                    return bracketIndexPair.Second;
            throw new InterpreterException("Curly brace is never closed");
        }

        private static int GetSingleSuperStatementEnd(string text, int headerEnd)
        {
            int coreStart = headerEnd + 1;
            if (coreStart >= text.Length)
                throw new InterpreterException("Substatement expected after if/else/while/for statement");
            string coreText = text[coreStart..];
            string[] statementsAfterHeader = GetStatementsAsStrings(coreText, 0, out _, out _);
            if (statementsAfterHeader.Length == 0)
                throw new InterpreterException("Substatement expected after if/else/while/for statement");
            string firstStatementAfterHeader = statementsAfterHeader[0];
            int firstStatementIndex = text.IndexOf(firstStatementAfterHeader, coreStart);
            int firstStatementLastIndex = firstStatementIndex + firstStatementAfterHeader.Length - 1;
            return firstStatementLastIndex;
        }

        private static SuperStatement GetOpenSuperStatement(string text, int index)
        {
            if (IsWordAtIndex(text, index, "if"))
                return SuperStatement.IfUnknown;
            if (IsWordAtIndex(text, index, "else"))
                return SuperStatement.Else;
            if (IsWordAtIndex(text, index, "while"))
                return SuperStatement.WhileUnknown;
            if (IsWordAtIndex(text, index, "for"))
                return SuperStatement.ForUnknown;
            return SuperStatement.None;
        }

        private static bool IsWordAtIndex(string text, int index, string word)
        {
            if (word.Length == 0)
                return false;
            if (index + word.Length - 1 >= text.Length)
                return false;
            for (int i = 0; i < word.Length; i++)
                if (text[index + i] != word[i])
                    return false;
            if (index + word.Length == text.Length)
                return true;
            char afterChar = text[index + word.Length];
            if (char.IsLetter(afterChar) || char.IsDigit(afterChar) || afterChar == '_')
                return false;
            return true;
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
                            if (i >= 2 && text[i - 1] == '\\' && text[i - 2] != '\\')
                                break;
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
    }
}
