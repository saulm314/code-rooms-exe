using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.Good
{
    public class StringArrayDeclInit : ITest
    {
        public StringArrayDeclInit(string pathNoExt)
        {
            PathNoExt = pathNoExt;
        }

        public string PathNoExt { get; init; }

        public Variable[][] Stack => 
            new[]
            {
                new Variable[]
                {
                    new(@string.Array, "strArr", null, false),
                    new(@string.Array, "strArr2", null, true),
                    new(@string.Array, "strArr3", 1, true),
                    new(@string.Array, "strArr4", 2, true),
                    new(@string.Array, "strArr5", 11, true)
                }
            };

        public Variable[] Heap =>
            new Variable[]
            {
                new(null),
                new(@int, 0),
                new(@int, 2),
                new(@string, null),
                new(@string, null),
                new(@int, 0),
                new(@int, 1),
                new(@char, 'a'),
                new(@int, 2),
                new(@char, 'a'),
                new(@char, 'b'),
                new(@int, 4),
                new(@string, null),
                new(@string, 5),
                new(@string, 6),
                new(@string, 8)
            };

        public Error Error { get; } = Error.None;
    }
}
