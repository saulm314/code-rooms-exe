using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.Good
{
    public class SingleIfMultiElse : ITest
    {
        public SingleIfMultiElse(string pathNoExt)
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
                    new(@int, "myInt2", 0, true),
                    new(@int, "myInt3", 2, true),
                    new(@int, "myInt4", 2, true)
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
