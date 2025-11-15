namespace CRECSharpInterpreter
{
    public class ArrayLength
    {
        public ArrayLength(Variable array)
        {
            Array = array;
        }

        public Variable Array { get; init; }
        public int Length
        {
            get
            {
                if (Array.Value == null)
                    throw new ArrayLengthException(this,
                        $"Cannot determine length of array \"{Array.Name}\" because its value is null");
                int heapIndex = (int)Array.Value;
                return Memory.Instance!.Heap.GetLength(heapIndex);
            }
        }

        public class ArrayLengthException : InterpreterException
        {
            public ArrayLengthException(ArrayLength arrayLength, string? message = null) : base(message)
            {
                this.arrayLength = arrayLength;
            }

            public ArrayLength arrayLength;
        }
    }
}
