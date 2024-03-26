using CRECSharpInterpreter.Collections.Generic;
using System.Collections.Generic;
using System;
using System.Linq;

namespace CRECSharpInterpreter
{
    public static class KeyStringSeparator
    {
        private static string[] nonKeywordKeyStrings = new string[]
        {
            "=",
            "{",
            "}",
            ",",
            "+",
            "-",
            "*",
            "/",
            "<",
            ">",
            "(",
            ")",
            "!",
            "&",
            "|",
            "^",
            "%",
            ";"
        };

        private static string[] nonKeywordKeyStringsContainingOtherKeyStrings = new string[]
        {
            "<=",
            ">=",
            "&&",
            "||",
            "==",
            "!="
        };

        public static string[] GetKeyStringsAsStrings(string text)
        {
            AltListLockLock1<string, string> altNonQuoteQuotes = GetAlternatingNonQuoteQuotes(text);
            List<string> keyStrings = new();
            for (int i = 0; i < altNonQuoteQuotes.Count; i++)
            {
                switch (i % 2)
                {
                    case 0:
                        List<string> quoteFreeKeyStrings = GetKeyStringsFromQuoteFreeText((string)altNonQuoteQuotes[i]!);
                        keyStrings.AddRange(quoteFreeKeyStrings);
                        break;
                    case 1:
                        keyStrings.Add((string)altNonQuoteQuotes[i]!);
                        break;
                }
            }
            return keyStrings.ToArray();
        }

        private static List<string> GetKeyStringsFromQuoteFreeText(string text)
        {
            text = PutSpacesBetweenKeyStrings(text);
            text = JoinRequiredKeyStringsTogether(text);
            List<string> keyStrings = SeparateKeyStrings(text);
            CombineCasts(keyStrings);
            return keyStrings;
        }

        private static AltListLockLock1<string, string> GetAlternatingNonQuoteQuotes(string text)
        {
            List<Pair<int, int>> quoteIndexPairs = LineSeparator.GetQuoteIndexPairs(text);
            if (quoteIndexPairs.Count == 0)
                return new(text);
            AltListLockLock1<string, string> altNonQuoteQuotes;
            string firstNonQuote = text[..quoteIndexPairs[0].First];
            altNonQuoteQuotes = new(firstNonQuote);
            for (int i = 0; i < quoteIndexPairs.Count - 1; i++)
            {
                Pair<int, int> currentQuoteIndexPair = quoteIndexPairs[i];
                Pair<int, int> nextQuoteIndexPair = quoteIndexPairs[i + 1];
                string quote = text[currentQuoteIndexPair.First..(currentQuoteIndexPair.Second + 1)];
                string nonQuote = text[(currentQuoteIndexPair.Second + 1)..nextQuoteIndexPair.First];
                altNonQuoteQuotes.Add(quote, nonQuote);
            }
            Pair<int, int> finalQuoteIndexPair = quoteIndexPairs[quoteIndexPairs.Count - 1];
            string finalQuote = text[finalQuoteIndexPair.First..(finalQuoteIndexPair.Second + 1)];
            string finalNonQuote = text[(finalQuoteIndexPair.Second + 1)..];
            altNonQuoteQuotes.Add(finalQuote, finalNonQuote);
            return altNonQuoteQuotes;
        }

        private static string PutSpacesBetweenKeyStrings(string text)
        {
            // ensure that every key string is surrounded by at least one space on either side
            foreach (string keyString in nonKeywordKeyStrings)
                text = text.Replace(keyString, $" {keyString} ");

            // but now key strings containing other key strings have been separated into two
            // e.g. "<=" has become " <  = "
            // therefore when looking for the congregate key strings, we instead look for the transformed version
            // that is, instead of "<=", we look for " <  = "
            TransformNonKeywordKeyStringsContainingOtherKeyStrings();

            // find the transformed key strings, remove all spaces from them,
            //      and finally add two spaces around them again to separate them from other key strings
            // so " <  = " becomes " <= "
            foreach (string keyString in nonKeywordKeyStringsContainingOtherKeyStrings)
                text = text.Replace(keyString, $" {keyString.Replace(" ", string.Empty)} ");
            
            return text;
        }

        private static bool _transformed = false;
        private static void TransformNonKeywordKeyStringsContainingOtherKeyStrings()
        {
            if (_transformed)
                return;
            _transformed = true;
            for (int i = 0; i < nonKeywordKeyStringsContainingOtherKeyStrings.Length; i++)
            {
                foreach (string smallKeyString in nonKeywordKeyStrings)
                {
                    string newBigKeyString =
                        nonKeywordKeyStringsContainingOtherKeyStrings[i]
                        .Replace(smallKeyString, $" {smallKeyString} ");
                    nonKeywordKeyStringsContainingOtherKeyStrings[i] = newBigKeyString;
                }
            }
        }

        private static string JoinRequiredKeyStringsTogether(string text)
        {
            text = RemoveWhiteSpaceSurroundingCharacter(text, '[', Direction.LeftRight);
            text = RemoveWhiteSpaceSurroundingCharacter(text, ']', Direction.Left);
            text = RemoveWhiteSpaceSurroundingCharacter(text, '.', Direction.LeftRight);
            text = RemoveWhiteSpaceBetweenCharacters(text, '[', ']');
            return text;
        }

        private static string RemoveWhiteSpaceSurroundingCharacter(string input, char character, Direction direction)
        {
            int i = 0;
            while (i < input.Length)
            {
                if (input[i] == character)
                    input = RemoveWhiteSpaceSurroundingIndex(input, i, direction, out i);
                i++;
            }
            return input;
        }

        private static string RemoveWhiteSpaceBetweenCharacters(string input, char char1, char char2)
        {
            int i = 0;
            int bracketsOpened = 0;
            int lastIndexOfChar2 = input.LastIndexOf(char2);
            while (i < input.Length)
            {
                if (input[i] == char1)
                {
                    bracketsOpened++;
                    i++;
                    continue;
                }
                if (input[i] == char2)
                {
                    bracketsOpened--;
                    if (bracketsOpened < 0)
                        bracketsOpened = 0;
                    i++;
                    continue;
                }
                if (bracketsOpened == 0)
                {
                    i++;
                    continue;
                }
                if (!char.IsWhiteSpace(input[i]))
                {
                    i++;
                    continue;
                }
                if (i >= lastIndexOfChar2)
                {
                    i++;
                    continue;
                }
                input = input.Remove(i, 1);
            }
            return input;
        }

        private enum Direction
        {
            Left,
            Right,
            LeftRight
        }

        private static string RemoveWhiteSpaceSurroundingIndex(string input, int index, Direction direction, out int newIndex)
        {
            newIndex = index;
            switch (direction)
            {
                case Direction.Left:
                    return RemoveWhiteSpaceLeftOfIndex(input, index, out newIndex);
                case Direction.Right:
                    return RemoveWhiteSpaceRightOfIndex(input, index);
                case Direction.LeftRight:
                    input = RemoveWhiteSpaceLeftOfIndex(input, index, out newIndex);
                    return RemoveWhiteSpaceRightOfIndex(input, newIndex);
                default:
                    throw new InterpreterException($"Internal exception: Invalid direction {direction}");
            }
        }

        private static string RemoveWhiteSpaceLeftOfIndex(string input, int index, out int newIndex)
        {
            newIndex = index;
            int i = newIndex - 1;
            while (i >= 0)
                if (char.IsWhiteSpace(input[i]))
                {
                    input = input.Remove(i, 1);
                    newIndex--;
                    i--;
                }
                else
                    break;
            return input;
        }

        private static string RemoveWhiteSpaceRightOfIndex(string input, int index)
        {
            int i = index + 1;
            while (i < input.Length)
                if (char.IsWhiteSpace(input[i]))
                    input = input.Remove(i, 1);
                else
                    break;
            return input;
        }

        private static List<string> SeparateKeyStrings(string text)
        {
            // remove all whitespace characters and separate everything else into arrays
            IEnumerable<string> keyStringsEnumerable = text
                .Split(default(char[]), StringSplitOptions.TrimEntries)
                .Where(str => !string.IsNullOrWhiteSpace(str));
            List<string> keyStrings = new();
            foreach (string keyString in keyStringsEnumerable)
                keyStrings.Add(keyString);
            return keyStrings;
        }

        private static void CombineCasts(List<string> keyStrings)
        {
            // combine bracket-type-bracket combinations into casts
            int i = 1;
            while (i < keyStrings.Count - 1)
            {
                string keyString = keyStrings[i];
                VarType? varType = VarType.GetVarType(keyString);
                if (varType == null)
                {
                    i++;
                    continue;
                }
                string previousKeyString = keyStrings[i - 1];
                string nextKeyString = keyStrings[i + 1];
                if (previousKeyString != "(" || nextKeyString != ")")
                {
                    i++;
                    continue;
                }
                keyStrings.RemoveAt(i);
                keyStrings.RemoveAt(i);
                keyStrings[i - 1] = $"({keyString})";
            }
        }
    }
}
