namespace CRECSharpInterpreter
{
    public class ArrayStringElement
    {
        public ArrayStringElement(Variable array, string arrayString, string stringString)
        {
            Array = array;
            ArrayIndexExpression = GetIndexExpression(arrayString);
            StringIndexExpression = GetIndexExpression(stringString);
        }

        private Expression GetIndexExpression(string stringInsideBraces)
        {
            string[] indexKeyStringsAsStrings = KeyString.GetKeyStringsAsStrings(stringInsideBraces);
            KeyString[] indexKeyStrings = new KeyString[indexKeyStringsAsStrings.Length];
            for (int i = 0; i < indexKeyStrings.Length; i++)
                indexKeyStrings[i] = new(indexKeyStringsAsStrings[i]);
            Expression indexExpression = new(indexKeyStrings);
            if (indexExpression._VarType != VarType.@int)
                throw new ArrayStringElementException(this, "Index must be an integer");
            return indexExpression;
        }

        public Variable Array { get; init; }
        public int ArrayIndex { get; set; }
        public Expression ArrayIndexExpression { get; init; }

        public int StringIndex { get; set; }
        public Expression StringIndexExpression { get; init; }

        public class ArrayStringElementException : InterpreterException
        {
            public ArrayStringElementException(ArrayStringElement? arrayStringElement, string? message = null) : base(message)
            {
                this.arrayStringElement = arrayStringElement;
            }

            public ArrayStringElement? arrayStringElement;
        }
    }
}
