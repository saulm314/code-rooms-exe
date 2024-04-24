using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.Good
{
    public class ArrayLength : ITest
    {
        public ArrayLength(string pathNoExt)
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
                    new(@int.Array, "intArr2", 2, true),
                    new(@int, "myInt", 0, true),
                    new(@int, "myInt2", 2, true)
                }
            };

        public Variable[] Heap =>
            new Variable[]
            {
                new(null),
                new(@int, 0),
                new(@int, 2),
                new(@int, 0),
                new(@int, 0)
            };

        public Error Error { get; } = Error.None;
    }
}
