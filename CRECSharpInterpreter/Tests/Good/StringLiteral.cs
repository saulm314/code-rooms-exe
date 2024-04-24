using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.Good
{
    public class StringLiteral : ITest
    {
        public StringLiteral(string pathNoExt)
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
                    new(@string, "myStr3", 4, true),
                    new(@string, "myStr4", 8, true),
                    new(@string, "myStr5", 15, true)
                }
            };

        public Variable?[] Heap =>
            new Variable?[]
            {
                new(null),
                new(@int, 0),
                new(@int, 1),
                new(@char, 'a'),
                new(@int, 3),
                new(@char, 'a'),
                new(@char, 'b'),
                new(@char, 'c'),
                new(@int, 6),
                new(@char, '\\'),
                new(@char, 'a'),
                new(@char, '\\'),
                new(@char, 'b'),
                new(@char, '\\'),
                new(@char, 'c'),
                new(@int, 18),
                new(@char, '\''),
                new(@char, ';'),
                new(@char, '{'),
                new(@char, '('),
                new(@char, '['),
                new(@char, '['),
                new(@char, ')'),
                new(@char, '}'),
                new(@char, ']'),
                new(@char, '\\'),
                new(@char, 'n'),
                new(@char, ' '),
                new(@char, '/'),
                new(@char, '/'),
                new(@char, ' '),
                new(@char, '/'),
                new(@char, '*'),
                new(@char, ' ')
            };

        public Error Error { get; } = Error.None;
    }
}
