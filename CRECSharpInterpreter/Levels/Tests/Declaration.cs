namespace CRECSharpInterpreter.Levels.Tests
{
    public class Declaration : ILevelTest
    {
        public int StarsAchieved(int cycle)
        {
            return cycle switch
            {
                0 => Memory.Instance!.IsDeclared("myInt") ? 1 : 0,
                1 => Memory.Instance!.IsDeclared("myChar") ? 1 : 0,
                _ => 0
            };
        }
    }
}
