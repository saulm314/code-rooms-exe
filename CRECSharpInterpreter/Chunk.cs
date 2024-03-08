using System;
using System.Collections.Generic;
using System.Linq;

namespace CRECSharpInterpreter
{
    public class Chunk
    {
        public Chunk(string text)
        {
            Text = text;
            string trimmedText = Text.TrimEnd();
            if (trimmedText[trimmedText.Length - 1] != ';')
                throw new ChunkException(this, "Semicolon on final line expected");

            string[] linesStr = GetLines();
            Lines = new Line[linesStr.Length];
            for (int i = 0; i < Lines.Length; i++)
                Lines[i] = new(linesStr[i]);

            //CompileLines();
        }

        public string Text { get; init; }

        public Line[] Lines { get; init; }

        public List<Variable> Variables { get; } = new();

        private string[] GetLines()
            =>
                Text
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Where(str => !string.IsNullOrWhiteSpace(str))
                .ToArray();

        /*private void CompileLines()
        {
            foreach (Line line in Lines)
            {
                switch (line._Type)
                {
                    case Line.Type.Declaration:
                        Variables.Add(line.DeclaredVariable);
                        break;
                    case Line.Type.DeclarationInitialisation:
                        Variables.Add(line.DeclaredVariable);

                        break;
                }
            }
        }*/

        public class ChunkException : Exception
        {
            public ChunkException(Chunk chunk, string message = null) : base(message)
            {
                this.chunk = chunk;
            }

            public Chunk chunk;
        }
    }
}
