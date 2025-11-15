using RealVariable = CRECSharpInterpreter.Variable;

namespace CRECSharpInterpreter.Levels.Tests
{
    public class GarbageCollection : ILevelTest
    {
        public int StarsAchieved(int cycle)
        {
            foreach (RealVariable? variable in Memory.Instance!.Heap)
                if (variable != null && variable._VarType != null)
                    return 0;
            return 1;
        }
    }
}
