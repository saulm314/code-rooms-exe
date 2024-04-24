using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.Good
{
    public class BooleanAnd : ITest
    {
        public BooleanAnd(string pathNoExt)
        {
            PathNoExt = pathNoExt;
        }

        public string PathNoExt { get; init; }

        public Variable[][] Stack => 
            new[]
            {
                new Variable[]
                {
                    new(@bool, "myB", true, true),
                    new(@bool, "myB2", false, true),
                    new(@bool, "myB3", false, true),
                    new(@bool, "myB4", false, true)
                }
            };

        public Variable?[] Heap =>
            new Variable?[]
            {
                new(null)
            };

        public Error Error { get; } = Error.None;
    }
}
