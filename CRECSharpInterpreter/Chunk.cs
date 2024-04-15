using System;
using System.Linq;

namespace CRECSharpInterpreter
{
    public class Chunk
    {
        public Chunk(string text, Mode mode)
        {
            Text = LineNumberUtils.AddNewlineSeparators(text);

            statementsStr = StatementSeparator.GetStatementsAsStrings(Text, 0, out LineNumberInfo[] lineNumberInfos);
            Statements = new Statement[statementsStr.Length];
            LineNumberInfos = lineNumberInfos;

            _ = new Memory(mode);

            if (Memory.Instance!._Mode == Mode.Compilation)
                for (int i = 0; i < Statements.Length; i++)
                    Statements[i] = new(statementsStr[i], LineNumberInfos[i]);
        }

        public string Text { get; init; }

        public Statement[] Statements { get; init; }
        public LineNumberInfo[] LineNumberInfos { get; init; }

        public int statementsDone = 0;
        public bool RunNextStatement()
        {
            if (statementsDone >= statementsStr.Length)
                return false;
            if (Statements[statementsDone] == null)
                Statements[statementsDone] = new(statementsStr[statementsDone], LineNumberInfos[statementsDone]);
            Statements[statementsDone].Execute();
            if (!Statements[statementsDone].Executed)
                return true;
            statementsDone++;
            return true;
        }

        private string[] statementsStr;

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
