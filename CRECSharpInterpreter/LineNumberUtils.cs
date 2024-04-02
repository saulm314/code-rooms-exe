using System.Collections.Generic;
using System.Globalization;

namespace CRECSharpInterpreter
{
    public static class LineNumberUtils
    {
        public const char separator = '\u001E';

        public static string AddNewlineSeparators(string text)
        {
            ushort currentLine = 1;
            text = text.Insert(0, GetSeparator(currentLine));
            currentLine++;
            int index = 5;
            while (index < text.Length)
            {
                if (text[index] != '\n')
                {
                    index++;
                    continue;
                }
                text = text.Insert(index + 1, GetSeparator(currentLine));
                currentLine++;
                index += 6;
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
            return separator + lineNumberStr;
        }

        public static ushort[] GetLineNumbers(string text)
        {
            List<ushort> lineNumbers = new();
            int index = 0;
            while (index < text.Length)
            {
                if (text[index] != separator)
                {
                    index++;
                    continue;
                }
                string lineNumberStr = text[(index + 1)..(index + 5)];
                ushort lineNumber = GetNumber(lineNumberStr);
                lineNumbers.Add(lineNumber);
                index += 6;
            }
            return lineNumbers.ToArray();
        }

        private static ushort GetNumber(string hexNumberStr)
        {
            if (hexNumberStr.Length != 4)
                throw new InterpreterException("Internal error");
            ushort hexNumber = ushort.Parse(hexNumberStr, NumberStyles.HexNumber);
            return hexNumber;
        }
    }
}
