namespace CRECSharpInterpreter
{
    public class ArrayConstruction
    {
        public ArrayConstruction(VarType varType, string stringInsideBraces)
        {
            _VarType = varType;
            string[] arrayLengthKeyStringsAsStrings = KeyString.GetKeyStringsAsStrings(stringInsideBraces);
            KeyString[] arrayLengthKeyStrings = new KeyString[arrayLengthKeyStringsAsStrings.Length];
            for (int i = 0; i < arrayLengthKeyStrings.Length; i++)
                arrayLengthKeyStrings[i] = new(arrayLengthKeyStringsAsStrings[i]);
            ArrayLengthExpression = new(arrayLengthKeyStrings);
            if (ArrayLengthExpression._VarType != VarType.@int)
                throw new ArrayConstructionException(this, "Array length must be an integer");
        }

        public VarType _VarType { get; init; }
        public int ArrayLength { get; set; }
        public Expression ArrayLengthExpression { get; init; }

        public class ArrayConstructionException : InterpreterException
        {
            public ArrayConstructionException(ArrayConstruction arrayConstruction, string? message = null) : base(message)
            {
                this.arrayConstruction = arrayConstruction;
            }

            public ArrayConstruction arrayConstruction;
        }
    }
}
