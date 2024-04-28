namespace CRECSharpInterpreter.Levels
{
    public class DummyLevelTest : ILevelTest
    {
        public bool HasPassed(int cycle) => false;
    }
}
