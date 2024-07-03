namespace CREInterpreter;

using System;
using System.Diagnostics;

public static class DebugError
{
    public static void WriteError(object? message)
    {
        ConsoleColor originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Debug.WriteLine(message);
        Console.ForegroundColor = originalColor;
    }

    public static void WriteWarning(object? message)
    {
        ConsoleColor originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Debug.WriteLine(message);
        Console.ForegroundColor = originalColor;
    }
}