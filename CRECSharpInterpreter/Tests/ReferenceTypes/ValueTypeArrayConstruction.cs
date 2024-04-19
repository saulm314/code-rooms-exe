using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.ReferenceTypes
{
    public class ValueTypeArrayConstruction : ITest
    {
        public string Path => @"ReferenceTypes\ValueTypeArrayConstruction";

        public Variable[][] Stack =>
            new[]
            {
                new Variable[]
                {
                    new(@int.Array,     "intArr",       1,  true),
                    new(@double.Array,  "doubleArr",    2,  true),
                    new(@int,           "length",       3,  true),
                    new(@char.Array,    "charArr",      5,  true)
                }
            };

        public Variable[] Heap =>
            new Variable[]
            {
                new(null),
                new(@int, 0),
                new(@int, 2),
                new(@double, 0.0),
                new(@double, 0.0),
                new(@int, 3),
                new(@char, '\0'),
                new(@char, '\0'),
                new(@char, '\0')
            };

        public Error Error => Error.None;
    }
}
