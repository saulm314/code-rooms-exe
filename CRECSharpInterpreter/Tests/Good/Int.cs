using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.Good
{
    public class Int : ITest
    {
        public Int(string pathNoExt)
        {
            PathNoExt = pathNoExt;
        }

        public string PathNoExt { get; init; }

        public Variable[][] Stack => 
            new[]
            {
                new Variable[]
                {
                    new(@int, "myInt", 0, false),
                    new(@int, "myInt2", 0, true),
                    new(@int, "myInt3", 5, true),
                    new(@int, "myInt4", 2147483647, true),
                    new(@int, "myInt5", -2147483647, true),
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
