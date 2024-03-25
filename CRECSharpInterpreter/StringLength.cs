namespace CRECSharpInterpreter
{
    public class StringLength
    {
        public StringLength(Variable @string)
        {
            String = @string;
        }

        public Variable String { get; init; }
        public int Length
        {
            get
            {
                if (String.Value == null)
                    throw new StringLengthException(this,
                        $"Cannot determine length of string \"{String.Name}\" because its value is null");
                int heapIndex = (int)String.Value;
                return Memory.Instance!.Heap.GetLength(heapIndex);
            }
        }

        public class StringLengthException : InterpreterException
        {
            public StringLengthException(StringLength? stringLength, string? message = null) : base(message)
            {
                this.stringLength = stringLength;
            }

            public StringLength? stringLength;
        }
    }
}
