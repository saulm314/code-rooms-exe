using System.Collections.Generic;
using System.Globalization;

namespace CRECSharpInterpreter
{
    public static class LineNumberUtils
    {
        public const char SEPARATOR = '\u001E';
        public const int SEPARATOR_LENGTH = 5;

        public static string AddNewlineSeparators(string text)
        {
            ushort currentLine = 1;
            text = text.Insert(0, GetSeparator(currentLine));
            currentLine++;
            int index = SEPARATOR_LENGTH;
            while (index < text.Length)
            {
                if (text[index] != '\n')
                {
                    index++;
                    continue;
                }
                text = text.Insert(index + 1, GetSeparator(currentLine));
                currentLine++;
                index += SEPARATOR_LENGTH;
            }
            return text;
        }

        // 4-digit hex number prefixed with 0s if required
        private static string GetNumberAsHexString(ushort number)
        {
            string numberStr = number.ToString("X");
            while (numberStr.Length < 4)
                numberStr = "0" + numberStr;
            return numberStr;
        }

        private static string GetSeparator(ushort lineNumber)
        {
            string lineNumberStr = GetNumberAsHexString(lineNumber);
            return SEPARATOR + lineNumberStr;
        }

        // if line number array is empty, then actualLineNumber cannot be determined
        //      and is returned as ushort.MaxValue
        public static ushort[] GetLineNumbers(string line, out ushort actualLineNumber)
        {
            List<ushort> lineNumbers = new();
            int firstNonWhiteSpaceIndex = LineSeparator.GetFirstNonWhiteSpaceIndexAfterIndex(line, -1);
            actualLineNumber = ushort.MaxValue;
            int index = 0;
            while (index < line.Length)
            {
                if (line[index] != SEPARATOR)
                {
                    index++;
                    continue;
                }
                string lineNumberStr = line[(index + 1)..(index + SEPARATOR_LENGTH)];
                ushort lineNumber = GetNumber(lineNumberStr);
                lineNumbers.Add(lineNumber);
                if (index < firstNonWhiteSpaceIndex)
                    actualLineNumber = lineNumber;
                index += SEPARATOR_LENGTH;
            }
            if (lineNumbers.Count > 0 && actualLineNumber == ushort.MaxValue)
                actualLineNumber = lineNumbers[0];
            return lineNumbers.ToArray();
        }

        public static string RemoveSeparators(string text)
        {
            int index = 0;
            while (index < text.Length)
            {
                if (text[index] != SEPARATOR)
                {
                    index++;
                    continue;
                }
                text = text.Remove(index, SEPARATOR_LENGTH);
            }
            return text;
        }

        private static ushort GetNumber(string hexNumberStr)
        {
            if (hexNumberStr.Length != SEPARATOR_LENGTH - 1)
                throw new InterpreterException("Internal error");
            ushort hexNumber = ushort.Parse(hexNumberStr, NumberStyles.HexNumber);
            return hexNumber;
        }

        public static string TrimStart(string text)
        {
            text = text.TrimStart();
            if (text.Length == 0)
                return text;
            if (text[0] != SEPARATOR)
                return text;
            text = text.Remove(0, SEPARATOR_LENGTH);
            return TrimStart(text);
        }

        public static string TrimEnd(string text)
        {
            text = text.TrimEnd();
            if (text.Length < SEPARATOR_LENGTH)
                return text;
            if (text[text.Length - SEPARATOR_LENGTH] != SEPARATOR)
                return text;
            text = text.Remove(text.Length - SEPARATOR_LENGTH, SEPARATOR_LENGTH);
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
