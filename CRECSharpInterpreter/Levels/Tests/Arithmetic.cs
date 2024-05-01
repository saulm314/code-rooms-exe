using RealVariable = CRECSharpInterpreter.Variable;

namespace CRECSharpInterpreter.Levels.Tests
{
    public class Arithmetic : ILevelTest
    {
        public int StarsAchieved(int cycle)
        {
            int count = 0;
            if (Star1(cycle))
                count++;
            if (Star2(cycle))
                count++;
            if (Star3(cycle))
                count++;
            return count;
        }

        private bool Star1(int cycle)
        {
            return cycle switch
            {
                0 => DoubleEquals((double?)Memory.Instance!.GetVariable("combinedArea")?.Value, 41.67),
                1 => DoubleEquals((double?)Memory.Instance!.GetVariable("combinedArea")?.Value, 50.73),
                2 => DoubleEquals((double?)Memory.Instance!.GetVariable("combinedArea")?.Value, 102.57),
                _ => false
            } || Star2(cycle);
        }

        private bool Star2(int cycle)
        {
            return cycle switch
            {
                0 => DoubleEquals((double?)Memory.Instance!.GetVariable("combinedArea")?.Value, 41.7),
                1 => DoubleEquals((double?)Memory.Instance!.GetVariable("combinedArea")?.Value, 50.7),
                2 => DoubleEquals((double?)Memory.Instance!.GetVariable("combinedArea")?.Value, 102.6),
                _ => false
            };
        }

        private bool Star3(int cycle)
        {
            return cycle switch
            {
                0 => (int?)Memory.Instance!.GetVariable("isOdd")?.Value == 1,
                1 => (int?)Memory.Instance!.GetVariable("isOdd")?.Value == 0,
                2 => (int?)Memory.Instance!.GetVariable("isOdd")?.Value == 1,
                _ => false
            };
        }

        private bool DoubleEquals(double? x, double y)
        {
            if (x == null)
                return false;
            if (-0.01 < x - y && x - y < 0.01)
                return true;
            return false;
        }
    }
}
