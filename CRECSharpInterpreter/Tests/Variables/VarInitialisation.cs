using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.Variables
{
    public class VarInitialisation : ITest
    {
        public string Path => @"Variables\VarInitialisation";

        public Variable[][] Stack =>
            new[]
            {
                new Variable[]
                {
                    new(@int,           "myInt",        5,      true),
                    new(@double,        "myDouble",     5.0,    true),
                    new(@bool,          "myBool",       true,  true),
                    new(@char,          "myChar",       'a',   true),
                    new(@int.Array,     "myIntArr",     1,   true),
                    new(@double.Array,  "myDoubleArr",  5,   true),
                    new(@bool.Array,    "myBoolArr",    11,   true),
                    new(@char.Array,    "myCharArr",    14,   true),
                    new(@string,        "myString",     17,   true),
                    new(@string.Array,  "myStringArr",  32,   true)
                }
            };

        public Variable[] Heap =>
            new Variable[]
            {
                new(null),
                new(@int, 3),
                new(@int, 0),
                new(@int, 0),
                new(@int, 0),
                new(@int, 5),
                new(@double, 0.0),
                new(@double, 0.0),
                new(@double, 0.0),
                new(@double, 0.0),
                new(@double, 0.0),
                new(@int, 2),
                new(@bool, false),
                new(@bool, true),
                new(@int, 2),
                new(@char, 'a'),
                new(@char, 'b'),
                new(@int, 5),
                new(@char, 'h'),
                new(@char, 'e'),
                new(@char, 'l'),
                new(@char, 'l'),
                new(@char, 'o'),
                new(@int, 2),
                new(@char, 'h'),
                new(@char, 'i'),
                new(@int, 5),
                new(@char, 'h'),
                new(@char, 'e'),
                new(@char, 'l'),
                new(@char, 'l'),
                new(@char, 'o'),
                new(@int, 2),
                new(@string, 23),
                new(@string, 26),
            };

        public Error Error => Error.None;
    }
}
