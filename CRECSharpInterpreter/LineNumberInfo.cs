namespace CRECSharpInterpreter
{
    public readonly struct LineNumberInfo
    {
        public LineNumberInfo(ushort lineNumber, ushort actualLineNumber, ushort endLineNumber)
        {
            LineNumber = lineNumber;
            ActualLineNumber = actualLineNumber;
            EndLineNumber = endLineNumber;
        }

        // the line number at which the line starts
        // (though there may be newline characters between this point and the first non-whitespace character)
        public ushort LineNumber { get; init; }

        // the line number at the point of the first non-whitespace character
        public ushort ActualLineNumber { get; init; }

        // the line number at the point of the final non-whitespace character
        public ushort EndLineNumber { get; init;}
    }
}
