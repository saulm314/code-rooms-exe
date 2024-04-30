using RealVariable = CRECSharpInterpreter.Variable;

namespace CRECSharpInterpreter.Levels.Tests
{
    public class Booleans : ILevelTest
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
            if (Star4(cycle))
                count++;
            return count;
        }

        private bool Star1(int cycle)
        {
            return cycle switch
            {
                0 => (bool?)Memory.Instance!.GetVariable("isOdd")?.Value == true,
                1 => (bool?)Memory.Instance!.GetVariable("isOdd")?.Value == false,
                2 => (bool?)Memory.Instance!.GetVariable("isOdd")?.Value == true,
                _ => false
            };
        }

        private bool Star2(int cycle)
        {
            return cycle switch
            {
                0 => (bool?)Memory.Instance!.GetVariable("isEven")?.Value == false,
                1 => (bool?)Memory.Instance!.GetVariable("isEven")?.Value == true,
                2 => (bool?)Memory.Instance!.GetVariable("isEven")?.Value == false,
                _ => false
            };
        }

        private bool Star3(int cycle)
        {
            return cycle switch
            {
                0 => (bool?)Memory.Instance!.GetVariable("bothAreOdd")?.Value == false &&
                    (bool?)Memory.Instance!.GetVariable("atLeastOneIsOdd")?.Value == true,
                1 => (bool?)Memory.Instance!.GetVariable("bothAreOdd")?.Value == false &&
                    (bool?)Memory.Instance!.GetVariable("atLeastOneIsOdd")?.Value == false,
                2 => (bool?)Memory.Instance!.GetVariable("bothAreOdd")?.Value == true &&
                    (bool?)Memory.Instance!.GetVariable("atLeastOneIsOdd")?.Value == true,
                _ => false
            };
        }

        private bool Star4(int cycle)
        {
            return cycle switch
            {
                0 => (bool?)Memory.Instance!.GetVariable("firstIsSmallerThanSecond")?.Value == false,
                1 => (bool?)Memory.Instance!.GetVariable("firstIsSmallerThanSecond")?.Value == true,
                2 => (bool?)Memory.Instance!.GetVariable("firstIsSmallerThanSecond")?.Value == false,
                _ => false
            };
        }
    }
}
