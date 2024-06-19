using RealVariable = CRECSharpInterpreter.Variable;

namespace CRECSharpInterpreter.Levels.Tests
{
    public class Casts : ILevelTest
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
                0 => (int?)Memory.Instance!.GetVariable("myDoubleConverted")?.Value == 5 &&
                    (double?)Memory.Instance!.GetVariable("myIntConverted")?.Value == 7.0,
                1 => (int?)Memory.Instance!.GetVariable("myDoubleConverted")?.Value == 8 &&
                    (double?)Memory.Instance!.GetVariable("myIntConverted")?.Value == 3.0,
                2 => (int?)Memory.Instance!.GetVariable("myDoubleConverted")?.Value == 2 &&
                    (double?)Memory.Instance!.GetVariable("myIntConverted")?.Value == 2.0,
                _ => false
            };
        }

        private bool Star2(int cycle)
        {
            return cycle switch
            {
                0 => (double?)Memory.Instance!.GetVariable("doubleWithoutDecimal")?.Value == 2.0,
                1 => (double?)Memory.Instance!.GetVariable("doubleWithoutDecimal")?.Value == 2.0,
                2 => (double?)Memory.Instance!.GetVariable("doubleWithoutDecimal")?.Value == 0.0,
                _ => false
            };
        }

        private bool Star3(int cycle)
        {
            return cycle switch
            {
                0 => (int?)Memory.Instance!.GetVariable("roundedNumber")?.Value == 1,
                1 => (int?)Memory.Instance!.GetVariable("roundedNumber")?.Value == 2,
                2 => (int?)Memory.Instance!.GetVariable("roundedNumber")?.Value == 33,
                _ => false
            };
        }

    }
}
