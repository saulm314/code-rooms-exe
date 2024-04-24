using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.Good
{
    public class ValueTypeArrayLiteral : ITest
    {
        public ValueTypeArrayLiteral(string pathNoExt)
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
                    new(@int.Array, "intArr3", 4, true)
                }
            };

        public Variable?[] Heap =>
            new Variable?[]
            {
                new(null),
                new(@int, 0),
                new(@int, 1),
                new(@int, 1),
                new(@int, 2),
                new(@int, 2),
                new(@int, 3)
            };

        public Error Error { get; } = Error.None;
    }
}
