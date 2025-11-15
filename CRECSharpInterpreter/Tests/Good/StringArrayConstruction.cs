using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.Good
{
    public class StringArrayConstruction : ITest
    {
        public StringArrayConstruction(string pathNoExt)
        {
            PathNoExt = pathNoExt;
        }

        public string PathNoExt { get; init; }

        public Variable[][] Stack => 
            new[]
            {
                new Variable[]
                {
                    new(@string.Array, "strArr", 1, true),
                    new(@string.Array, "strArr2", 2, true)
                }
            };

        public Variable?[] Heap =>
            new Variable?[]
            {
                new(null),
                new(@int, 0),
                new(@int, 2),
                new(@string, null),
                new(@string, null)
            };

        public Error Error { get; } = Error.None;
    }
}
