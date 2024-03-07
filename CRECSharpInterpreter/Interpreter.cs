using System;
using System.Linq;

namespace CRECSharpInterpreter
{
    public class Interpreter
    {
        public Interpreter(string text)
        {
            Console.WriteLine($"Creating interpreter for the following text:\n\n{text}\n");
            chunk = new(text);
        }

        public Chunk chunk;

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

        public bool InterpretNextLine()
        {
            throw new NotImplementedException();
        }
    }
}
