using System;

namespace CRECSharpInterpreter
{
    public class Interpreter
    {
        internal Interpreter(string text, bool dummy)
        {
            Console.WriteLine($"Creating interpreter for the following text:\n\n{text}");
            chunk = new(text, Mode.Compilation);
            Console.ReadLine();
            Console.WriteLine(SEPARATOR + "\n");

            chunk = new(text, Mode.Runtime);
            while (chunk.RunNextStatement())
            {
                Console.ReadLine();
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
                    Memory.Instance.Frames.Add(new());
            }
            catch (InterpreterException e)
            {
                Memory.Instance.Thrown = true;
                Memory.Instance.ThrownException = e;
            }
            Memory.Instance.Frames[Memory.Instance.CurrentFrame].Init();
        }

        public Chunk chunk;

        public const string SEPARATOR = "____________________________________________";
    }
}
