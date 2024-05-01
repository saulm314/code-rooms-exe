using RealVariable = CRECSharpInterpreter.Variable;

namespace CRECSharpInterpreter.Levels.Tests
{
    public class ReadWriteVariables : ILevelTest
    {
        public int StarsAchieved(int cycle)
        {
            int count = 0;
            if (Star1())
                count++;
            if (Star2(cycle))
                count++;
            return count;
        }

        private bool Star1()
        {
            return Memory.Instance!.GetVariable("iAmTrue")?.Initialised ?? false;
        }

        private bool Star2(int cycle)
        {
            RealVariable? variable = Memory.Instance!.GetVariable("number2");
            if (variable == null)
                return false;
            return cycle switch
            {
                0 => (double?)variable.Value == 3.2,
                1 => (double?)variable.Value == 5.7,
                2 => (double?)variable.Value == 63.9,
                _ => false
            };
        }
    }
}
