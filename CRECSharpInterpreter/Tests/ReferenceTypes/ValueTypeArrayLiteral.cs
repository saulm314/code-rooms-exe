using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.ReferenceTypes
{
    public class ValueTypeArrayLiteral : ITest
    {
        public string Path => @"ReferenceTypes\ValueTypeArrayLiteral";

        public Variable[][] Stack =>
            new[]
            {
                new Variable[]
                {
                    new(@int.Array, "intArr", 1, true),
                    new(@double, "first", 3.0, true),
                    new(@double, "second", -3.9, true),
                    new(@double.Array, "doubleArr", 4, true),
                    new(@char.Array, "charArr", 9, true),
                    new(@bool.Array, "boolArr", 10, true),
                }
            };

        public Variable[] Heap =>
            new Variable[]
            {
                new(null),
                new(@int, 2),
                new(@int, 5),
                new(@int, 3),
                new(@int, 4),
                new(@double, 3.0),
                new(@double, -3.9),
                new(@double, -0.3),
                new(@double, 3.0),
                new(@int, 0),
                new(@int, 1),
                new(@bool, false)
            };

        public Error Error => Error.None;
    }
}
