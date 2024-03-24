namespace CRECSharpInterpreter.Operators
{
    public readonly struct Operand
    {
        public Operand(VarType varType) => _VarType = varType;

        public VarType _VarType { get; init; }
    }
}
