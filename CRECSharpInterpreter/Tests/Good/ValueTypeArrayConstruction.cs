using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.Good
{
    public class ValueTypeArrayConstruction : ITest
    {
        public ValueTypeArrayConstruction(string pathNoExt)
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
                    new(@double.Array, "doubleArr", 2, true)
                }
            };

        public Variable?[] Heap =>
            new Variable?[]
            {
                new(null),
                new(@int, 0),
                new(@int, 2),
                new(@double, 0.0),
                new(@double, 0.0)
            };

        public Error Error { get; } = Error.None;
    }
}
