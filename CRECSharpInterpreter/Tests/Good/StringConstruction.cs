using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.Good
{
    public class StringConstruction : ITest
    {
        public StringConstruction(string pathNoExt)
        {
            PathNoExt = pathNoExt;
        }

        public string PathNoExt { get; init; }

        public Variable[][] Stack => 
            new[]
            {
                new Variable[]
                {
                    new(@char.Array, "charArr", 1, true),
                    new(@string, "myStr", 4, true),
                    new(@string, "myStr2", 7, true),
                    new(@string, "myStr3", 10, true)
                }
            };

        public Variable[] Heap =>
            new Variable[]
            {
                new(null),
                new(@int, 2),
                new(@char, 'a'),
                new(@char, 'b'),
                new(@int, 2),
                new(@char, 'a'),
                new(@char, 'b'),
                new(@int, 2),
                new(@char, 'x'),
                new(@char, 'y'),
                new(@int, 2),
                new(@char, '\0'),
                new(@char, '\0')
            };

        public Error Error { get; } = Error.None;
    }
}
