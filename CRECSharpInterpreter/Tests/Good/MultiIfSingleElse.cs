using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.Good
{
    public class MultiIfSingleElse : ITest
    {
        public MultiIfSingleElse(string pathNoExt)
        {
            PathNoExt = pathNoExt;
        }

        public string PathNoExt { get; init; }

        public Variable[][] Stack => 
            new[]
            {
                new Variable[]
                {
                    new(@int, "myInt", 1, true),
                    new(@int, "myInt2", 1, true),
                    new(@int, "myInt3", 2, true),
                    new(@int, "myInt4", 0, true)
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
