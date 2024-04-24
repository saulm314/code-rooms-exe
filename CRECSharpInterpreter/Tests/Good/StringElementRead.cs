using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.Good
{
    public class StringElementRead : ITest
    {
        public StringElementRead(string pathNoExt)
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
                    new(@char, "myChar", 'a', true),
                    new(@char, "myChar2", 'b', true),
                    new(@char, "myChar3", 'c', true)
                }
            };

        public Variable[] Heap =>
            new Variable[]
            {
                new(null),
                new(@int, 3),
                new(@char, 'a'),
                new(@char, 'b'),
                new(@char, 'c')
            };

        public Error Error { get; } = Error.None;
    }
}
