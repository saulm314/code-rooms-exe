using System;

namespace CRECSharpInterpreter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string dummyText = "int _number = 1;\nint secondNumber = _number;\nsecondNumber = 3;\n_number = secondNumber;\nsecondNumber=_number;\n" +
                "int thirdNumber = _number;\nint numbernumber = -193214;\nthirdNumber = numbernumber;";
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
