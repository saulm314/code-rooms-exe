using System;
using System.Linq;

namespace CRECSharpInterpreter
{
    public class Line
    {
        public Line(string text)
        {
            Text = text;
            string[] keyStringsStr = GetKeyStrings();
            KeyStrings = new KeyString[keyStringsStr.Length];
            for (int i = 0; i < KeyStrings.Length; i++)
                KeyStrings[i] = new(keyStringsStr[i]);
        }

        public string Text { get; init; }

        public KeyString[] KeyStrings { get; init; }

        // needs to be updated as new components are added
        private static string[] nonKeywordKeyStrings = new string[]
        {
            "="
        };

        // needs to be updated as new components are added
        private string[] GetKeyStrings()
        {
            // ensure that every key string is surrounded by at least one space on either side
            string parsedText = Text;
            foreach (string keyString in nonKeywordKeyStrings)
                parsedText = parsedText.Replace(keyString, $" {keyString} ");

            // remove all whitespace characters and return everything else, separated
            return
                parsedText
                .Split(default(char[]), StringSplitOptions.TrimEntries)
                .Where(str => !string.IsNullOrWhiteSpace(str))
                .ToArray();
        }
    }
}
