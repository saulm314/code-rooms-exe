namespace CRECSharpInterpreter.Tests
{
    public interface ITest
    {
        string Path { get; }
        Variable[][] Stack { get; }
        Variable[] Heap { get; }
        Error Error { get; }
    }
}
