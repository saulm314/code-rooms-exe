namespace CRECSharpInterpreter.Levels.Tests
{
    public class Declaration : ILevelTest
    {
        public bool HasPassed(int cycle)
        {
            return cycle switch
            {
                0 => Memory.Instance!.IsDeclared("myInt"),
                1 => Memory.Instance!.IsDeclared("myChar"),
                _ => false
            };
        }
    }
}
