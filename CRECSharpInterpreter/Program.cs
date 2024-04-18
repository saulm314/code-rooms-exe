using static CRECSharpInterpreter.Console;
using System;
using System.IO;
using System.Threading;

namespace CRECSharpInterpreter
{
    public class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "test")
                Environment.Testing = true;
            string[] languages = new string[] { "Java", "C#" };
            WriteLine("Choose syntax: [1-2]");
            WriteLine("\t1) Java");
            WriteLine("\t2) C#");
            Write("> ");
            string? choice = ReadLine();
            try
            {
                Environment._Syntax = choice switch
                {
                    "1" => Syntax.Java,
                    "2" => Syntax.CSharp,
                    _ => throw new Exception()
                };
            }
            catch
            {
                WriteLine($"WARNING: Unrecognised choice \"{choice}\"; choosing default option\n");
            }
            WriteLine($"Setting syntax to {languages[(int)Environment._Syntax]}");
            WriteLine();
            Environment.Debug = true;
            ReadLine();
            string fileName = Environment._Syntax switch
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
                WriteLine(e);
            }
            Thread.Sleep(10000);
            for (;;)
                ReadLine();
        }
    }
}
