using System;
using System.IO;

namespace CRECSharpInterpreter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            StreamReader streamReader = new(@"..\..\..\..\Files\Dummy.cs");
            string dummyText = streamReader.ReadToEnd();
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
