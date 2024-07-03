using System;
using System.Diagnostics;
using System.IO;

namespace CREInterpreter;

public class Program
{
    private static void Main(string[] args)
    {
        Trace.Listeners.Add(new ConsoleTraceListener());
        Debug.WriteLine("Running CREInterpreter in debug mode with args:");
        foreach (string arg in args)
            Debug.WriteLine($"\t{arg}");
        Debug.WriteLine(null);

        #if DEBUG
        string dummyText = File.ReadAllText($@"..\..\..\..\Files\Dummy.cs");
        #elif RELEASE
        string dummyText = File.ReadAllText($@"Files\Dummy.cs");
        #endif
    }
}