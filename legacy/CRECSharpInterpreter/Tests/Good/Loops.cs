using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.Good
{
    public class Loops : ITest
    {
        public Loops(string pathNoExt)
        {
            PathNoExt = pathNoExt;
        }

        public string PathNoExt { get; init; }

        public Variable[][] Stack => 
            new[]
            {
                new Variable[]
                {
                    new(@bool, "whileTest", false, true),
                    new(@int, "whileTest2", 0, true),
                    new(@int.Array, "copyArray", 1, true),
                    new(@int.Array, "pasteArray", 5, true),
                    new(@int, "index", 3, true),
                    new(@char.Array, "helloChars", 9, true),
                    new(@char.Array, "helloCharsCopy", 15, true),
                    new(@bool, "arraysAreEqual", true, true),
                    new(@char.Array, "helloBackwards", 21, true),
                    new(@bool, "breakTest", true, true),
                    new(@int, "continueTest", 0, true),
                    new(@int, "toInitialise", 5, true),
                    new(@int, "anInt", 9, true)
                }
            };

        public Variable?[] Heap =>
            new Variable?[]
            {
                new(null),
                new(@int, 3),
                new(@int, 1),
                new(@int, 2),
                new(@int, 3),
                new(@int, 3),
                new(@int, 1),
                new(@int, 2),
                new(@int, 3),
                new(@int, 5),
                new(@char, 'h'),
                new(@char, 'e'),
                new(@char, 'l'),
                new(@char, 'l'),
                new(@char, 'o'),
                new(@int, 5),
                new(@char, 'h'),
                new(@char, 'e'),
                new(@char, 'l'),
                new(@char, 'l'),
                new(@char, 'o'),
                new(@int, 5),
                new(@char, 'o'),
                new(@char, 'l'),
                new(@char, 'l'),
                new(@char, 'e'),
                new(@char, 'h'),
            };

        public Error Error { get; } = Error.None;
    }
}
