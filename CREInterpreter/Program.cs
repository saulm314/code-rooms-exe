namespace CREInterpreter;

using System;
using System.Diagnostics;
using System.IO;

public class Program
{
    private static void Main(string[] args)
    {
        Trace.Listeners.Add(new ConsoleTraceListener());
        Debug.WriteLine("Running CREInterpreter in debug mode with args:");
        foreach (string arg in args)
            Debug.WriteLine($"\t{arg}");
        Debug.WriteLine(null);

        Console.WriteLine("Choose syntax: [1-2]");
        Console.WriteLine("\t1) C#");
        Console.WriteLine("\t2) Java");
        Console.Write("> ");
        string? choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                Environment.Instance._Syntax = Syntax.CSharp;
                break;
            case "2":
                Environment.Instance._Syntax = Syntax.Java;
                break;
            default:
                ConsoleError.WriteWarning($"Unrecognised choice {choice}, setting default value");
                break;
        }
        Console.WriteLine($"Setting syntax to {Environment.Instance._Syntax}");
        Console.WriteLine();
        Console.ReadLine();

        string fileName = Environment.Instance._Syntax switch
        {
            Syntax.CSharp => "Dummy.cs",
            Syntax.Java => "Dummy.java",
            _ => throw new Exception("internal error")
        };
        #if DEBUG
        string dummyText = File.ReadAllText($@"..\..\..\..\Files\{fileName}");
        #elif RELEASE
        string dummyText = File.ReadAllText($@"Files\{fileName}");
        #endif
    }
}