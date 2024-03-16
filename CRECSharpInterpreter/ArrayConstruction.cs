namespace CRECSharpInterpreter
{
    public class ArrayConstruction
    {
        public ArrayConstruction(VarType varType, int arrayLength)
        {
            _VarType = varType;
            ArrayLength = arrayLength;
        }

        public VarType _VarType { get; init; }
        public int ArrayLength { get; init; }

        public class ArrayConstructionException : InterpreterException
        {
            public ArrayConstructionException(ArrayConstruction arrayConstruction, string message = null) : base(message)
            {
                this.arrayConstruction = arrayConstruction;
            }

            public ArrayConstruction arrayConstruction;
        }
    }
}
