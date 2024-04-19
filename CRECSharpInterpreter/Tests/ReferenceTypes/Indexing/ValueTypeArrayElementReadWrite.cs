using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.ReferenceTypes.Indexing
{
    public class ValueTypeArrayElementReadWrite : ITest
    {
        public string Path => @"ReferenceTypes\Indexing\ValueTypeArrayElementReadWrite";

        public Variable[][] Stack =>
            new[]
            {
                new Variable[]
                {
                    new(@int.Array,     "intArr",           1,  true),
                    new(@int,           "myInt",            3,  true),
                    new(@int,           "myInt2",           6,  true),
                    new(@int,           "myInt3",           9,  true),
                    new(@int.Array,     "reverseIntArr",    5,  true)
                }
            };

        public Variable[] Heap =>
            new Variable[]
            {
                new(null),
                new(@int, 3),
                new(@int, 3),
                new(@int, 6),
                new(@int, 9),
                new(@int, 3),
                new(@int, 9),
                new(@int, 6),
                new(@int, 3),
            };

        public Error Error => Error.None;
    }
}
