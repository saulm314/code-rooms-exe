namespace CRECSharpInterpreter
{
    public class ArrayElement
    {
        public ArrayElement(Variable array, string stringInsideBraces)
        {
            Array = array;
            string[] indexKeyStringsAsStrings = KeyString.GetKeyStringsAsStrings(stringInsideBraces);
            KeyString[] indexKeyStrings = new KeyString[indexKeyStringsAsStrings.Length];
            for (int i = 0; i < indexKeyStrings.Length; i++)
                indexKeyStrings[i] = new(indexKeyStringsAsStrings[i]);
            IndexExpression = new(indexKeyStrings);
            if (IndexExpression._VarType != VarType.@int)
                throw new ArrayElementException(this, "Array index must be an integer");
        }

        public Variable Array { get; init; }
        public int Index { get; set; }
        public Expression IndexExpression { get; init; }

        public class ArrayElementException : InterpreterException
        {
            public ArrayElementException(ArrayElement arrayElement, string message = null) : base(message)
            {
                this.arrayElement = arrayElement;
            }

            public ArrayElement arrayElement;
        }
    }
}
