using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.ReferenceTypes
{
    public class ValueTypeArrayDeclInit : ITest
    {
        public string Path => @"ReferenceTypes\ValueTypeArrayDeclInit";

        public Variable[][] Stack =>
            new[]
            {
                new Variable[]
                {
                    new(@int.Array,     "intArr",       null,   false),
                    new(@double.Array,  "doubleArr",    null,   false),
                    new(@bool.Array,    "boolArr",      null,   false),
                    new(@char.Array,    "charArr",      null,   false),
                    new(@int.Array,     "intArr2",      null,   true),
                    new(@double.Array,  "doubleArr2",   1,      true),
                    new(@bool.Array,    "boolArr2",     2,      true),
                    new(@char.Array,    "charArr2",     5,      true),
                }
            };

        public Variable[] Heap =>
            new Variable[]
            {
                new(null),
                new(@int, 0),
                new(@int, 2),
                new(@bool, false),
                new(@bool, false),
                new(@int, 3),
                new(@char, 'a'),
                new(@char, 'b'),
                new(@char, 'c'),
            };

        public Error Error => Error.None;
    }
}
