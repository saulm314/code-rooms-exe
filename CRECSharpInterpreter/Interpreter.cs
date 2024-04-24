using static CRECSharpInterpreter.Console;
using CRECSharpInterpreter.Tests;
using System;

namespace CRECSharpInterpreter
{
    public class Interpreter
    {
        internal Interpreter(string text, bool dummy)
        {
            WriteLine("Store all memory frames? (y/n)\n");
            Write("> ");
            string? storeAllFramesStr = ReadLine();
            bool storeAllFrames = storeAllFramesStr == "y";
            WriteLine();

            if (!storeAllFrames)
            {
                WriteLine($"Creating interpreter for the following text:\n\n{text}");
                chunk = new(text, Mode.Compilation);
                ReadLine();
                WriteLine(SEPARATOR + "\n");
                chunk = new(text, Mode.Runtime);
                while (chunk.RunNextStatement())
                {
                    ReadLine();
                }
                WriteLine("Done");
                return;
            }

            string? input = null;
            WriteLine("Controls:");
            WriteLine("\tExit: \"exit\"");
            WriteLine("\tMove right: \"\"");
            WriteLine("\tMove left: \"*\"");
            WriteLine();
            WriteLine("Press Enter to continue");
            ReadLine();
            WriteLine();
            WriteLine($"Creating interpreter for the following text:\n\n{text}");
            chunk = new(text, Mode.Compilation);
            chunk = new(text, Mode.RuntimeStoreAllFrames);
            while (true)
            {
                input = ReadLine();
                if (input == "exit")
                    break;
                if (input == string.Empty)
                {
                    if (Memory.Instance!.Frames[Memory.Instance.CurrentFrame].CanMoveRight)
                    {
                        bool isLast = Memory.Instance.CurrentFrame == Memory.Instance.Frames.Count - 1;
                        MoveRight();
                        if (!isLast || Memory.Instance.Thrown)
                            PrintFrame();
                    }
                    else
                        WriteLine("Already all the way right");
                }
                else
                {
                    if (Memory.Instance!.Frames[Memory.Instance.CurrentFrame].CanMoveLeft)
                    {
                        MoveLeft();
                        PrintFrame();
                    }
                    else
                        WriteLine("Already all the way left");
                }
            }
            WriteLine("Done");
        }

        internal Interpreter(string text, byte dummy)
        {
            chunk = new(string.Empty, Mode.Compilation);
            try
            {
                chunk = new(text, Mode.Compilation);
            }
            catch (Exception e)
            {
                error = Error.Compile;
                exception = e;
                return;
            }
            chunk = new(text, Mode.Runtime);
        }

        internal void RunAll()
        {
            try
            {
                while (chunk.RunNextStatement()) { }
            }
            catch (Exception e)
            {
                error = Error.Run;
                exception = e;
            }
        }

        internal Error error = Error.None;
        internal Exception? exception;

        public Interpreter(string text)
        {
            chunk = new(text, Mode.Compilation);
            chunk = new(text, Mode.RuntimeStoreAllFrames);
        }

        // the MoveLeft and MoveRight methods are only compatible if the mode is RuntimeStoreAllFrames
        // it is the caller's responsibility to check that the move left or right is valid
        // this can be found in the CanMoveLeft and CanMoveRight properties of the current MemoryFrame

        public void MoveLeft()
        {
            Memory.Instance!.CurrentFrame--;
        }

        public void MoveRight()
        {
            if (Memory.Instance!.CurrentFrame < Memory.Instance.Frames.Count - 1)
            {
                Memory.Instance.CurrentFrame++;
                return;
            }
            try
            {
                Memory.Instance.Executed = !chunk.RunNextStatement();
                if (Memory.Instance.Executed)
                {
                    Memory.Instance.Frames.Add(new());
                    Memory.Instance.Frames[Memory.Instance.CurrentFrame].Init();
                }
            }
            catch (InterpreterException e)
            {
                Memory.Instance.Thrown = true;
                Memory.Instance.ThrownException = e;
                Memory.Instance.Frames[Memory.Instance.CurrentFrame].Init();
            }
            catch (Exception e) when (Environment.Debug)
            {
                Memory.Instance.Thrown = true;
                Memory.Instance.ThrownException = e;
                Memory.Instance.Frames[Memory.Instance.CurrentFrame].Init();
            }
        }

        public Chunk chunk;

        public const string SEPARATOR = "____________________________________________";

        private MemoryFrame Frame => Memory.Instance!.Frames[Memory.Instance.CurrentFrame];
        private void PrintFrame()
        {
            WriteLine(Frame.Statement?.ReducedText + '\n');
            if (Memory.Instance!.CurrentFrame == Memory.Instance.Frames.Count - 1 && Memory.Instance!.Thrown)
                WriteLine(Memory.Instance.ThrownException!);
            WriteLine("Stack:");
            WriteLine(SEPARATOR + "\n");
            Scope[] scopes = Frame.Stack?.ToArray() ?? Array.Empty<Scope>();

            for (int i = scopes.Length - 1; i >= 0; i--)
            {
                Scope scope = scopes[i];
                foreach (Variable variable in scope.DeclaredVariables)
                    WriteLine(variable);
                WriteLine(SEPARATOR + "\n");
            }
            WriteLine("\nHeap:\n");
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                    Write(string.Format("{0,10}", Frame.Heap?[10 * i + j]?.ValueAsString ?? "x"));
                Write("\n");
            }
            WriteLine();
            WriteLine(SEPARATOR + SEPARATOR + SEPARATOR);
        }
    }
}
