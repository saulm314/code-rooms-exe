using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.Good
{
    public class GreaterThanOrEqualTo : ITest
    {
        public GreaterThanOrEqualTo(string pathNoExt)
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
                    new(@bool, "myB2", true, true),
                    new(@bool, "myB3", true, true),
                    new(@bool, "myB4", false, true),
                    new(@bool, "myB5", true, true),
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
