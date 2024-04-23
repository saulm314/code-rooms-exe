using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.Good
{
    public class GreaterThan : ITest
    {
        public GreaterThan(string pathNoExt)
        {
            PathNoExt = pathNoExt;
        }

        public string PathNoExt { get; init; }

        public Variable[][] Stack => 
            new[]
            {
                new Variable[]
                {
                    new(@bool, "myB", false, true),
                    new(@bool, "myB2", false, true),
                    new(@bool, "myB3", true, true),
                    new(@bool, "myB4", false, true),
                    new(@bool, "myB5", false, true),
                    new(@bool, "myB6", true, true)
                }
            };

        public Variable[] Heap =>
            new Variable[]
            {
                new(null)
            };

        public Error Error { get; } = Error.None;
    }
}
