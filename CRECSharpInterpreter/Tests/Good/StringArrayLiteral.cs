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
                    new(@string.Array, "strArr3", 8, true),
                    new(@char.Array, "charArr", 11, true),
                    new(@string.Array, "strArr4", 29, true)
                }
            };

        public Variable?[] Heap =>
            new Variable?[]
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
                new(@string, 6),
                new(@int, 3),
                new(@char, 'x'),
                new(@char, 'y'),
                new(@char, 'z'),
                new(@int, 2),
                new(@char, '\0'),
                new(@char, '\0'),
                new(@int, 2),
                new(@char, 'a'),
                new(@char, 'b'),
                new(@int, 3),
                new(@char, 'x'),
                new(@char, 'y'),
                new(@char, 'z'),
                new(@int, 3),
                new(@char, 'p'),
                new(@char, 'q'),
                new(@char, 'r'),
                new(@int, 4),
                new(@string, 15),
                new(@string, 18),
                new(@string, 21),
                new(@string, 25)
            };

        public Error Error { get; } = Error.None;
    }
}
