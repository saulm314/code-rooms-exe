namespace CRECSharpInterpreter
{
    public interface IExpressionComponent
    {
        public ExpressionComponentType _Type { get; }
        public KeyString[] GetKeyStrings();
    }
}
