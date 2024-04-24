using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.Good
{
    public class StringArrayLiteral : ITest
    {
        public StringArrayLiteral(string pathNoExt)
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
                    new(@string.Array, "strArr2", 3, true),
                    new(@string.Array, "strArr3", 8, true)
                }
            };

        public Variable[] Heap =>
            new Variable[]
            {
                new(null),
                new(@int, 0),
                new(@int, 0),
                new(@int, 1),
                new(@string, 2),
                new(@int, 0),
                new(@int, 1),
                new(@char, 'a'),
                new(@int, 2),
                new(@string, 5),
                new(@string, 6)
            };

        public Error Error { get; } = Error.None;
    }
}
