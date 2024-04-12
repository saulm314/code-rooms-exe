using System;

namespace CRECSharpInterpreter
{
    public class Interpreter
    {
        public Interpreter(string text)
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

        public Chunk chunk;

        public const string SEPARATOR = "____________________________________________";
    }
}
