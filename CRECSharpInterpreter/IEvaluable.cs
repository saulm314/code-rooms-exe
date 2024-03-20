namespace CRECSharpInterpreter
{
    public interface IEvaluable
    {
        public VarType _VarType { get; }
        public object Value { get; }

        public void Compute();

        public KeyString[] GetKeyStrings();
    }
}
