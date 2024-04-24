using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.Good
{
    public class GarbageCollection : ITest
    {
        public GarbageCollection(string pathNoExt)
        {
            PathNoExt = pathNoExt;
        }

        public string PathNoExt { get; init; }

        public Variable[][] Stack => 
            new[]
            {
                new Variable[]
                {
                    new(@int.Array, "intArr", null, true),
                    new(@int.Array, "intArr2", null, true),
                    new(@int.Array, "intArr3", 1, true),
                    new(@int.Array, "intArr4", null, true),
                    new(@int.Array, "intArr5", null, true),
                    new(@string, "myStr", null, true),
                    new(@string, "myStr2", 4, true),
                    new(@string.Array, "animals", null, true),
                    new(@string, "animal", 12, true),
                    new(@string.Array, "animals2", null, true),
                    new(@string, "animal2", null, true),
                    new(@string.Array, "animals3", 8, true),
                    new(@string, "animal3", 16, true),
                    new(@string, "animal4", null, true),
                    new(@string.Array, "animals4", 10, true),
                    new(@string, "animal5", null, true),
                    new(@string.Array, "animals5", null, true),
                    new(@char.Array, "charArr", null, true),
                    new(@string, "abc", 30, true),
                    new(@string, "xyz", 26, true),
                    new(@string.Array, "pqr", 39, true),
                    new(@bool, "check", false, true)
                }
            };

        public Variable?[] Heap =>
            new Variable?[]
            {
                new(null),
                new(@int, 2),
                new(@int, 7),
                new(@int, 8),
                new(@int, 3),
                new(@char, 'B'),
                new(@char, 'y'),
                new(@char, 'e'),
                new(@int, 1),
                new(@string, null),
                new(@int, 1),
                new(@string, 22),
                new(@int, 3),
                new(@char, 'D'),
                new(@char, 'o'),
                new(@char, 'g'),
                new(@int, 5),
                new(@char, 'M'),
                new(@char, 'o'),
                new(@char, 'o'),
                new(@char, 's'),
                new(@char, 'e'),
                new(@int, 3),
                new(@char, 'B'),
                new(@char, 'a'),
                new(@char, 't'),
                new(@int, 3),
                new(@char, 'x'),
                new(@char, 'y'),
                new(@char, 'z'),
                new(@int, 3),
                new(@char, 'a'),
                new(@char, 'b'),
                new(@char, 'c'),
                new(@int, 2),
                new(@char, 'p'),
                new(@char, 'q'),
                new(@int, 1),
                new(@char, 'r'),
                new(@int, 2),
                new(@string, 34),
                new(@string, 37)
            };

        public Error Error { get; } = Error.None;
    }
}
