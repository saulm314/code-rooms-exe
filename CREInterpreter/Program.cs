namespace CREInterpreter;

using System.Diagnostics;

public class Program
{
    private static void Main(string[] args)
    {
        Trace.Listeners.Add(new ConsoleTraceListener());
        Debug.WriteLine("Hello");
    }
}