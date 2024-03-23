namespace CRECSharpInterpreter
{
    public interface IEvaluable : IExpressionComponent
    {
        public VarType _VarType { get; }
        public object Value { get; }

        public void Compute();

        ExpressionComponentType IExpressionComponent._Type { get => ExpressionComponentType.Evaluable; }
    }
}
