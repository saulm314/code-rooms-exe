namespace CRECSharpInterpreter.Levels.Tests
{
    public class Declaration : ILevelTest
    {
        public bool HasPassed(int cycle)
        {
            return
                Memory.Instance!.IsDeclared("myInt");
        }
    }
}
