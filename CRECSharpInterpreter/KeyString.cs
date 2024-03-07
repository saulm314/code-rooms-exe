using System;

namespace CRECSharpInterpreter
{
    public class KeyString
    {
        public KeyString(string text)
        {
            Text = text;
            Console.WriteLine(text);
        }

        public string Text { get; init; }
    }
}
