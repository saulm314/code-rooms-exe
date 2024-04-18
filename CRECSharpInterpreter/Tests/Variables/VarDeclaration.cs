using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.Variables
{
    public class VarDeclaration : ITest
    {
        public string Path => @"Variables\VarDeclaration";

        public Variable[][] Stack =>
            new[]
            {
                new Variable[]
                {
                    new(@int,           "myInt",        0,      false),
                    new(@double,        "myDouble",     0.0,    false),
                    new(@bool,          "myBool",       false,  false),
                    new(@char,          "myChar",       '\0',   false),
                    new(@int.Array,     "myIntArr",     null,   false),
                    new(@double.Array,  "myDoubleArr",  null,   false),
                    new(@bool.Array,    "myBoolArr",    null,   false),
                    new(@char.Array,    "myCharArr",    null,   false),
                    new(@string,        "myString",     null,   false),
                    new(@string.Array,  "myStringArr",  null,   false)
                }
            };

        public Variable[] Heap =>
            new Variable[]
            {
                new(null)
            };

        public Error Error => Error.None;
    }
}
