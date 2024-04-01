using System;
using System.IO;

namespace CRECSharpInterpreter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string fileName = SyntaxEnvironment._Syntax switch
            {
                Syntax.CSharp => "Dummy.cs",
                Syntax.Java => "Dummy.java",
                _ => throw new Exception("internal error")
            };
            StreamReader streamReader = new(@$"..\..\..\..\Files\{fileName}");
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
