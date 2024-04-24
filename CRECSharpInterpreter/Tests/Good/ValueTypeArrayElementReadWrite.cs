using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.Good
{
    public class ValueTypeArrayElementReadWrite : ITest
    {
        public ValueTypeArrayElementReadWrite(string pathNoExt)
        {
            PathNoExt = pathNoExt;
        }

        public string PathNoExt { get; init; }

        public Variable[][] Stack => 
            new[]
            {
                new Variable[]
                {
                    new(@int.Array, "intArr", 1, true),
                    new(@int, "myInt", 3, true),
                    new(@int, "myInt2", 6, true),
                    new(@int, "myInt3", 9, true),
                    new(@int.Array, "reverseIntArr", 5, true)
                }
            };

        public Variable[] Heap =>
            new Variable[]
            {
                new(null),
                new(@int, 3),
                new(@int, 3),
                new(@int, 6),
                new(@int, 9),
                new(@int, 3),
                new(@int, 9),
                new(@int, 6),
                new(@int, 3)
            };

        public Error Error { get; } = Error.None;
    }
}
