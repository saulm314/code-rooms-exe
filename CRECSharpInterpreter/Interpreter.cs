using System;

namespace CRECSharpInterpreter
{
    public class Interpreter
    {
        internal Interpreter(string text, bool dummy)
        {
            Console.WriteLine($"Creating interpreter for the following text:\n\n{text}");
            chunk = new(text, Mode.Compilation);
            Console.ReadLine();
            Console.WriteLine(SEPARATOR + "\n");

            chunk = new(text, Mode.Runtime);
            while (chunk.RunNextStatement())
            {
                Console.ReadLine();
            }
            Console.WriteLine("Done");
        }

        public Interpreter(string text)
        {
            chunk = new(text, Mode.Compilation);
            chunk = new(text, Mode.Runtime);
        }

        public Chunk chunk;

        public const string SEPARATOR = "____________________________________________";
    }
}
