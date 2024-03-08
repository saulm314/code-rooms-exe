using System;

namespace CRECSharpInterpreter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string dummyText = "int number;\nnumber = 1;";
            try
            {
                Interpreter interpreter = new(dummyText);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            while (true) { }
        }
    }
}
