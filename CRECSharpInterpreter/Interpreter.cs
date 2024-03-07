using System;
using System.Linq;

namespace CRECSharpInterpreter
{
    public class Interpreter
    {
        public Interpreter(string text)
        {
            this.text = text;
            Console.WriteLine($"Interpreter for the following text created:\n\n{text}\n");
        }

        public readonly string text;

        public void Initialise()
        {
            Console.WriteLine("Initialising interpreter");

            lines = GetLinesFromText(text);

            foreach (string line in lines)
                ParseLine(line);
        }

        /*
        Possible lines:
            * int number;               declaration
            * int number = 0;           declaration + initialisation
            * number = 0;               initialisation
            * number = 0;               write
            * number = otherNumber;     read
        
        Supported data types:
            * int
        */

        private string[] lines;

        // needs to be updated as new components are added
        private string[] GetLinesFromText(string text)
        {
            return
                text
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Where(str => !string.IsNullOrWhiteSpace(str))
                .ToArray();
        }

        private void ParseLine(string line)
        {
            string[] keyStrings = GetKeyStringsFromLine(line);
            foreach (string keyString in keyStrings)
                Console.WriteLine($"Key string: {keyString}");
        }

        // needs to be updated as new components are added
        private static string[] nonKeywordKeyStrings = new string[]
        {
            "="
        };

        // needs to be updated as new components are added
        private string[] GetKeyStringsFromLine(string line)
        {
            // ensure that every key string is surrounded by at least one space on either side
            foreach (string keyString in nonKeywordKeyStrings)
                line = line.Replace(keyString, $" {keyString} ");

            // remove all whitespace characters and return everything else, separated
            return
                line
                .Split(default(char[]), StringSplitOptions.TrimEntries)
                .Where(str => !string.IsNullOrWhiteSpace(str))
                .ToArray();
        }

        public bool InterpretNextLine()
        {
            throw new NotImplementedException();
        }
    }
}
