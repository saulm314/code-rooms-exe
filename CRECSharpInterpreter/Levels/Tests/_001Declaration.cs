namespace CRECSharpInterpreter.Levels.Tests
{
    public class _001Declaration : ILevelTest
    {
        public bool HasPassed(int cycle)
        {
            return
                Memory.Instance!.IsDeclared("myInt");
        }
    }
}
