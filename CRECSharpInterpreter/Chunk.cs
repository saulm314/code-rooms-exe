using System.Linq;

namespace CRECSharpInterpreter
{
    public class Chunk
    {
        public Chunk(string text)
        {
            Text = text;
            string[] linesStr = GetLines();
            Lines = new Line[linesStr.Length];
            for (int i = 0; i < Lines.Length; i++)
                Lines[i] = new(linesStr[i]);
        }

        public string Text { get; init; }

        public Line[] Lines { get; init; }

        // needs to be updated as new components are added
        private string[] GetLines()
            =>
                Text
                .Split(';', System.StringSplitOptions.RemoveEmptyEntries)
                .Where(str => !string.IsNullOrWhiteSpace(str))
                .ToArray();
    }
}
