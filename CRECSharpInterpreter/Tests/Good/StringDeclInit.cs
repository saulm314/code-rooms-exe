using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.Good
{
    public class StringDeclInit : ITest
    {
        public StringDeclInit(string pathNoExt)
        {
            PathNoExt = pathNoExt;
        }

        public string PathNoExt { get; init; }

        public Variable[][] Stack => 
            new[]
            {
                new Variable[]
                {
                    new(@string, "myStr", null, false),
                    new(@string, "myStr2", null, true),
                    new(@string, "myStr3", 1, true),
                    new(@char.Array, "charArr", 3, true),
                    new(@string, "myStr4", 6, true)
                }
            };

        public Variable[] Heap =>
            new Variable[]
            {
                new(null),
                new(@int, 1),
                new(@char, 'A'),
                new(@int, 2),
                new(@char, 'a'),
                new(@char, 'b'),
                new(@int, 2),
                new(@char, 'a'),
                new(@char, 'b')
            };

        public Error Error { get; } = Error.None;
    }
}
