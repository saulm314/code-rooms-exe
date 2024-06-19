using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.Good
{
    public class StringArrayElementReadWrite : ITest
    {
        public StringArrayElementReadWrite(string pathNoExt)
        {
            PathNoExt = pathNoExt;
        }

        public string PathNoExt { get; init; }

        public Variable[][] Stack => 
            new[]
            {
                new Variable[]
                {
                    new(@string.Array, "strArr", 6, true),
                    new(@string, "myStr", null, true),
                    new(@string, "myStr2", 1, true),
                    new(@string, "myStr3", 3, true),
                    new(@string.Array, "reverseStrArr", 10, true)
                }
            };

        public Variable?[] Heap =>
            new Variable?[]
            {
                new(null),
                new(@int, 1),
                new(@char, 'a'),
                new(@int, 2),
                new(@char, 'a'),
                new(@char, 'b'),
                new(@int, 3),
                new(@string, null),
                new(@string, 1),
                new(@string, 3),
                new(@int, 3),
                new(@string, 3),
                new(@string, 1),
                new(@string, null)
            };

        public Error Error { get; } = Error.None;
    }
}
