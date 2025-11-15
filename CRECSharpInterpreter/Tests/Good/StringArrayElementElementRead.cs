using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.Good
{
    public class StringArrayElementElementRead : ITest
    {
        public StringArrayElementElementRead(string pathNoExt)
        {
            PathNoExt = pathNoExt;
        }

        public string PathNoExt { get; init; }

        public Variable[][] Stack => 
            new[]
            {
                new Variable[]
                {
                    new(@string.Array, "strArr", 9, true),
                    new(@char, "myChar", 'a', true),
                    new(@char, "myChar2", 'c', true),
                    new(@char, "myChar3", 'x', true),
                    new(@char, "myChar4", 'z', true)
                }
            };

        public Variable?[] Heap =>
            new Variable?[]
            {
                new(null),
                new(@int, 3),
                new(@char, 'a'),
                new(@char, 'b'),
                new(@char, 'c'),
                new(@int, 3),
                new(@char, 'x'),
                new(@char, 'y'),
                new(@char, 'z'),
                new(@int, 2),
                new(@string, 1),
                new(@string, 5)
            };

        public Error Error { get; } = Error.None;
    }
}
