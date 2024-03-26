namespace CRECSharpInterpreter
{
    public class HeapLengthVariable : Variable
    {
        public HeapLengthVariable() : base(VarType.@int) { }

        public int referenceCount = 1;
    }
}
