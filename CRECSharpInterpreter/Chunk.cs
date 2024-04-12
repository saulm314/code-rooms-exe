using System;
using System.Linq;

namespace CRECSharpInterpreter
{
    public class Chunk
    {
        public Chunk(string text, Mode mode)
        {
            Text = LineNumberUtils.AddNewlineSeparators(text);

            linesStr = LineSeparator.GetLinesAsStrings(Text, 0, out LineNumberInfo[] lineNumberInfos);
            Lines = new Line[linesStr.Length];
            LineNumberInfos = lineNumberInfos;

            _ = new Memory(mode);

            if (Memory.Instance!._Mode == Mode.Compilation)
                for (int i = 0; i < Lines.Length; i++)
                    Lines[i] = new(linesStr[i], LineNumberInfos[i]);
        }

        public string Text { get; init; }

        public Line[] Lines { get; init; }
        public LineNumberInfo[] LineNumberInfos { get; init; }

        private int linesDone = 0;
        public bool RunNextLine()
        {
            if (linesDone >= linesStr.Length)
                return false;
            if (Lines[linesDone] == null)
                Lines[linesDone] = new(linesStr[linesDone], LineNumberInfos[linesDone]);
            Lines[linesDone].Execute();
            if (!Lines[linesDone].Executed)
                return true;
            linesDone++;
            return true;
        }

        private string[] linesStr;

        public class ChunkException : InterpreterException
        {
            public ChunkException(Chunk chunk, string? message = null) : base(message)
            {
                this.chunk = chunk;
            }

            public Chunk chunk;
        }
    }
}
