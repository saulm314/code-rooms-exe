using System;
using System.Linq;

namespace CRECSharpInterpreter
{
    public class Chunk
    {
        public Chunk(string text, Mode mode)
        {
            Text = text;
            string trimmedText = Text.TrimEnd();
            if (trimmedText[trimmedText.Length - 1] != ';')
                throw new ChunkException(this, "Semicolon on final line expected");

            linesStr = GetLines();
            Lines = new Line[linesStr.Length];

            _ = new Memory(mode);

            if (Memory.Instance._Mode == Mode.Compilation)
                for (int i = 0; i < Lines.Length; i++)
                    Lines[i] = new(linesStr[i]);
        }

        public string Text { get; init; }

        public Line[] Lines { get; init; }

        private int linesDone = 0;
        public bool RunNextLine()
        {
            if (linesDone >= linesStr.Length)
                return false;
            Lines[linesDone] = new(linesStr[linesDone]);
            linesDone++;
            return true;
        }

        private string[] linesStr;

        private string[] GetLines()
            =>
                Text
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Where(str => !string.IsNullOrWhiteSpace(str))
                .ToArray();

        public class ChunkException : InterpreterException
        {
            public ChunkException(Chunk chunk, string message = null) : base(message)
            {
                this.chunk = chunk;
            }

            public Chunk chunk;
        }
    }
}
