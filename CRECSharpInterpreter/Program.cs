using System;
using System.IO;
using System.Threading;

namespace CRECSharpInterpreter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string[] languages = new string[] { "Java", "C#" };
            Console.WriteLine("Choose syntax: [1-2]");
            Console.WriteLine("\t1) Java");
            Console.WriteLine("\t2) C#");
            Console.Write("> ");
            string? choice = Console.ReadLine();
            try
            {
                SyntaxEnvironment._Syntax = choice switch
                {
                    "1" => Syntax.Java,
                    "2" => Syntax.CSharp,
                    _ => throw new Exception()
                };
            }
            catch
            {
                Console.WriteLine($"WARNING: Unrecognised choice \"{choice}\"; choosing default option\n");
            }
            Console.WriteLine($"Setting syntax to {languages[(int)SyntaxEnvironment._Syntax]}");
            Console.WriteLine();
            Console.ReadLine();
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
                Interpreter interpreter = new(dummyText, false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Thread.Sleep(10000);
            for (;;)
                Console.ReadLine();
            
        }
    }
}
