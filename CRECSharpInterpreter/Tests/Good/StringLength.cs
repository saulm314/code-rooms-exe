using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.Good
{
    public class StringLength : ITest
    {
        public StringLength(string pathNoExt)
        {
            PathNoExt = pathNoExt;
        }

        public string PathNoExt { get; init; }

        public Variable[][] Stack => 
            new[]
            {
                new Variable[]
                {
                    new(@string, "myStr", 1, true),
                    new(@string, "myStr2", 2, true),
                    new(@int, "myInt", 0, true),
                    new(@int, "myInt2", 2, true)
                }
            };

        public Variable?[] Heap =>
            new Variable?[]
            {
                new(null),
                new(@int, 0),
                new(@int, 2),
                new(@char, 'a'),
                new(@char, 'b')
            };

        public Error Error { get; } = Error.None;
    }
}
