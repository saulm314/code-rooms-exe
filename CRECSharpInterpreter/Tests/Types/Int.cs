using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.Types
{
    public class Int : ITest
    {
        public string Path => @"Types\Int";

        public Variable[][] Stack =>
            new[]
            {
                new Variable[]
                {
                    new(@int, "myInt",  0,              true),
                    new(@int, "myInt2", 5,              true),
                    new(@int, "myInt3", 2147483647,     true),
                    new(@int, "myInt4", -2147483647,    true),
                }
            };

        public Variable[] Heap =>
            new Variable[]
            {
                new(null)
            };

        public Error Error => Error.None;
    }
}
