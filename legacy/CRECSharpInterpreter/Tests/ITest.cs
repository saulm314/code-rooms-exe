namespace CRECSharpInterpreter.Tests
{
    public interface ITest
    {
        string PathNoExt { get; }
        Variable[][] Stack { get; }
        Variable?[] Heap { get; }
        Error Error { get; }
    }
}
