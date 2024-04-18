using System;

namespace CRECSharpInterpreter
{
    public class Interpreter
    {
        internal Interpreter(string text, bool dummy)
        {
            Console.WriteLine("Store all memory frames? (y/n)\n> ");
            string? storeAllFramesStr = Console.ReadLine();
            bool storeAllFrames = storeAllFramesStr == "y";
            Console.WriteLine();

            if (!storeAllFrames)
            {
                Console.WriteLine($"Creating interpreter for the following text:\n\n{text}");
                chunk = new(text, Mode.Compilation);
                Console.WriteLine(SEPARATOR + "\n");
                chunk = new(text, Mode.Runtime);
                while (chunk.RunNextStatement())
                {
                    Console.ReadLine();
                }
                Console.WriteLine("Done");
                return;
            }

            string? input = null;
            Console.WriteLine("Controls:");
            Console.WriteLine("\tExit: \"exit\"");
            Console.WriteLine("\tMove right: \"\"");
            Console.WriteLine("\tMove left: \"*\"");
            Console.WriteLine();
            Console.WriteLine("Press Enter to continue");
            Console.ReadLine();
            Console.WriteLine();
            Console.WriteLine($"Creating interpreter for the following text:\n\n{text}");
            chunk = new(text, Mode.Compilation);
            chunk = new(text, Mode.RuntimeStoreAllFrames);
            while (true)
            {
                input = Console.ReadLine();
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
                        Console.WriteLine("Already all the way right");
                }
                else
                {
                    if (Memory.Instance!.Frames[Memory.Instance.CurrentFrame].CanMoveLeft)
                    {
                        MoveLeft();
                        PrintFrame();
                    }
                    else
                        Console.WriteLine("Already all the way left");
                }
            }
            Console.WriteLine("Done");
        }

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
            catch (Exception e) when (Environment._Debug == Debug.Yes)
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
            Console.WriteLine(Frame.Statement?.ReducedText + '\n');
            if (Memory.Instance!.CurrentFrame == Memory.Instance.Frames.Count - 1 && Memory.Instance!.Thrown)
                Console.WriteLine(Memory.Instance.ThrownException!);
            Console.WriteLine("Stack:");
            Console.WriteLine(SEPARATOR + "\n");
            Scope[] scopes = Frame.Stack?.ToArray() ?? Array.Empty<Scope>();

            for (int i = scopes.Length - 1; i >= 0; i--)
            {
                Scope scope = scopes[i];
                foreach (Variable variable in scope.DeclaredVariables)
                    Console.WriteLine(variable);
                Console.WriteLine(SEPARATOR + "\n");
            }
            Console.WriteLine("\nHeap:\n");
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                    Console.Write(string.Format("{0,10}", Frame.Heap?[10 * i + j]?.ValueAsString ?? "x"));
                Console.Write("\n");
            }
            Console.WriteLine();
            Console.WriteLine(SEPARATOR + SEPARATOR + SEPARATOR);
        }
    }
}
