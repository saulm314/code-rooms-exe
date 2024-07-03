﻿namespace CREInterpreter;

using System;
using System.Diagnostics;

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
                Console.Error.WriteLine($"Unrecognised choice {choice}, setting default value");
                break;
        }
        Console.WriteLine($"Setting syntax to {Environment.Instance._Syntax}");
        Console.WriteLine();
        Console.ReadLine();


    }
}