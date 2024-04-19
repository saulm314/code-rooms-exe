using static CRECSharpInterpreter.Console;
using CRECSharpInterpreter.Collections.Generic;
using CRECSharpInterpreter.Tests;
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
            {
                bool verbose = args.Length > 1 && args[1] == "verbose";
                RunTests(verbose);
                return;
            }
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

        private static void RunTests(bool verbose)
        {
            System.Console.WriteLine();
            Environment.Testing = true;
            foreach (ITest test in TestRepository.Tests)
                RunTest(test, verbose);
            System.Console.WriteLine($"{TestRepository.SuccessfulTests}/{TestRepository.TotalTests} passed");
        }

        private static readonly Pair<Syntax, string>[] languages = new Pair<Syntax, string>[]
        {
            new(Syntax.CSharp, ".cs"),
            new(Syntax.Java, ".java")
        };

        private static void RunTest(ITest test, bool verbose)
        {
            System.Console.WriteLine($"{test.Path}:");
            foreach (Pair<Syntax, string> language in languages)
            {
                Environment._Syntax = language.First;
                string fullPath = @"..\..\..\..\Files\Tests\" + test.Path + language.Second;
                string text = File.ReadAllText(fullPath);
                Interpreter interpreter = new(text, 0);
                bool?[] results = new bool?[2];
                if (verbose)
                    System.Console.WriteLine($"Compile (test vs real): {test.Error} {interpreter.error} {interpreter.exception}");
                switch (test.Error, interpreter.error)
                {
                    case (Error.None, Error.None):
                        results[0] = true;
                        break;
                    case (Error.None, Error.Compile):
                        results[0] = false;
                        results[1] = false;
                        break;
                    case (Error.None, Error.Run):
                        throw new InterpreterException("Internal error");
                    case (Error.Compile, Error.None):
                        results[0] = false;
                        results[1] = false;
                        break;
                    case (Error.Compile, Error.Compile):
                        results[0] = true;
                        results[1] = true;
                        break;
                    case (Error.Compile, Error.Run):
                        throw new InterpreterException("Internal error");
                    case (Error.Run, Error.None):
                        results[0] = true;
                        break;
                    case (Error.Run, Error.Compile):
                        results[0] = false;
                        results[1] = false;
                        break;
                    case (Error.Run, Error.Run):
                        throw new InterpreterException("Internal error");
                }
                if (results[1] != null)
                {
                    PrintResults(test, language, results);
                    continue;
                }
                interpreter.RunAll();
                if (verbose)
                    System.Console.WriteLine($"Run (test vs real): {test.Error} {interpreter.error} {interpreter.exception}");
                switch (test.Error, interpreter.error)
                {
                    case (Error.None, Error.None):
                        break;
                    case (Error.None, Error.Compile):
                        throw new InterpreterException("Internal error");
                    case (Error.None, Error.Run):
                        results[1] = false;
                        break;
                    case (Error.Compile, Error.None):
                    case (Error.Compile, Error.Compile):
                    case (Error.Compile, Error.Run):
                        throw new InterpreterException("Internal error");
                    case (Error.Run, Error.None):
                        results[1] = false;
                        break;
                    case (Error.Run, Error.Compile):
                        throw new InterpreterException("Internal error");
                    case (Error.Run, Error.Run):
                        results[1] = true;
                        break;
                }
                if (results[1] != null)
                {
                    PrintResults(test, language, results);
                    continue;
                }
                if (verbose)
                    PrintMemory();
                results[1] = DoesMemoryMatch(test, verbose);
                PrintResults(test, language, results);
            }
            System.Console.WriteLine();
        }

        private static void PrintResults(ITest test, Pair<Syntax, string> language, bool?[] results)
        {
            foreach (bool? result in results)
            {
                TestRepository.SuccessfulTests += (bool)result! ? 1 : 0;
                TestRepository.TotalTests++;
            }
            System.Console.WriteLine(string.Format("{0,20} {1,10}", language.First + "_Compile", results[0]));
            System.Console.WriteLine(string.Format("{0,20} {1,10}", language.First + "_Run", results[1]));
        }

        private static bool DoesMemoryMatch(ITest test, bool verbose)
        {
            if (!DoesStackMatch(test.Stack, verbose))
            {
                if (verbose)
                    System.Console.WriteLine("Test stack does not match real stack");
                return false;
            }
            if (!DoesHeapMatch(test.Heap, verbose))
            {
                if (verbose)
                    System.Console.WriteLine("Test heap does not match real heap");
                return false;
            }
            return true;
        }

        private static bool DoesStackMatch(Variable[][] stack, bool verbose)
        {
            if (stack.Length != Memory.Instance!.Stack.Count)
            {
                if (verbose)
                    System.Console.WriteLine($"Test stack size {stack.Length} different from real stack size {Memory.Instance.Stack.Count}");
                return false;
            }
            Scope[] stack2 = Memory.Instance!.Stack.ToArray();
            Array.Reverse(stack2);
            for (int i = 0; i < stack.Length; i++)
                if (!DoesVarArrayMatch(stack[i], stack2[i], verbose))
                {
                    if (verbose)
                        System.Console.WriteLine($"Scope {i} does not match");
                    return false;
                }
            return true;
        }

        private static bool DoesVarArrayMatch(Variable[] vars, Scope scope2, bool verbose)
        {
            if (vars.Length != scope2.DeclaredVariables.Count)
            {
                if (verbose)
                    System.Console.WriteLine($"Test scope size {vars.Length} doesn't match real scope size {scope2.DeclaredVariables.Count}");
                return false;
            }
            for (int i = 0; i < vars.Length; i++)
                if (!DoesVarMatch(vars[i], scope2.DeclaredVariables[i], verbose))
                {
                    if (verbose)
                        System.Console.WriteLine($"Stack variable {i} does not match");
                    return false;
                }
            return true;
        }

        private static bool DoesVarMatch(Variable variable, Variable? variable2, bool verbose)
        {
            if (variable2 == null)
            {
                if (verbose)
                    System.Console.WriteLine("Real variable is null (the variable itself, not its value");
                return false;
            }
            bool varTypeMatch = variable._VarType == variable2._VarType;
            bool nameMatch = variable.Name == variable2.Name;
            bool valueMatch = Equals(variable.Value, variable2.Value);
            bool initialisedMatch = variable.Initialised == variable2.Initialised;
            if (!varTypeMatch)
            {
                if (verbose)
                    System.Console.WriteLine($"Test varType {variable._VarType} doesn't match real varType {variable2._VarType}");
                return false;
            }
            if (!nameMatch)
            {
                if (verbose)
                    System.Console.WriteLine($"Test name {variable.Name} doesn't match real name {variable2.Name}");
                return false;
            }
            if (!valueMatch)
            {
                if (verbose)
                    System.Console.WriteLine($"Test value {variable.Value} doesn't match real value {variable2.Value}");
                return false;
            }
            if (!initialisedMatch)
            {
                if (verbose)
                    System.Console.WriteLine($"Test initialised {variable.Initialised} doesn't match real initialised {variable2.Initialised}");
                return false;
            }
            return true;
        }

        private static bool DoesHeapMatch(Variable[] heap, bool verbose)
        {
            if (heap.Length > Memory.Instance!.Heap.Size)
            {
                if (verbose)
                    System.Console.WriteLine($"Test heap ({heap.Length}) bigger than real heap ({Memory.Instance.Heap.Size})");
                return false;
            }
            for (int i = 0; i < heap.Length; i++)
                if (!DoesVarMatch(heap[i], Memory.Instance.Heap[i], verbose))
                {
                    if (verbose)
                        System.Console.WriteLine($"Heap variable {i} does not match");
                    return false;
                }
            for (int i = heap.Length; i < Memory.Instance.Heap.Size; i++)
                if (Memory.Instance.Heap[i] != null)
                {
                    if (verbose)
                        System.Console.WriteLine($"Heap variable {i} null expected");
                    return false;
                }
            return true;
        }

        private static void PrintMemory()
        {
            Scope[] stack = Memory.Instance!.Stack.ToArray();
            Array.Reverse(stack);
            System.Console.WriteLine("Stack:\n");
            foreach (Scope scope in stack)
                for (int i = 0; i < scope.DeclaredVariables.Count; i++)
                {
                    Variable variable = scope.DeclaredVariables[i];
                    System.Console.WriteLine(i + "\t" + variable);
                }
            System.Console.WriteLine();
            System.Console.WriteLine("Heap:\n");
            for (int i = 0; i < Memory.Instance.Heap.Size; i++)
            {
                Variable? variable = Memory.Instance.Heap[i];
                System.Console.WriteLine(i + "\t" + (variable?.ToString() ?? "x"));
            }
        }
    }
}
