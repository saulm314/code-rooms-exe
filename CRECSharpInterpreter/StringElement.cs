namespace CRECSharpInterpreter
{
    public class StringElement
    {
        public StringElement(Variable @string, string stringInsideBraces)
        {
            String = @string;
            string[] indexKeyStringsAsStrings = KeyStringSeparator.GetKeyStringsAsStrings(stringInsideBraces);
            KeyString[] indexKeyStrings = new KeyString[indexKeyStringsAsStrings.Length];
            for (int i = 0; i < indexKeyStrings.Length; i++)
                indexKeyStrings[i] = new(indexKeyStringsAsStrings[i]);
            IndexExpression = new(indexKeyStrings);
            if (IndexExpression._VarType != VarType.@int)
                throw new StringElementException(this, "String index must be an integer");
        }

        public Variable String { get; init; }
        public int Index { get; set; }
        public Expression IndexExpression { get; init; }

        public class StringElementException : InterpreterException
        {
            public StringElementException(StringElement? stringElement, string? message = null) : base(message)
            {
                this.stringElement = stringElement;
            }

            public StringElement? stringElement;
        }
    }
}
