using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.ValueTypes
{
    public class Int : ITest
    {
        public string Path => @"ValueTypes\Int";

        public Variable[][] Stack =>
            new[]
            {
                new Variable[]
                {
                    new(@int, "myInt",  0,              true),
                    new(@int, "myInt2", 5,              true),
                    new(@int, "myInt3", 2147483647,     true),
                    new(@int, "myInt4", -2147483647,    true),
                    new(@int, "myInt5", 0,              false),
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
