using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.ValueTypes
{
    public class Double : ITest
    {
        public string Path => @"ValueTypes\Double";

        public Variable[][] Stack =>
            new[]
            {
                new Variable[]
                {
                    new(@double, "myDouble",  0.0,     true),
                    new(@double, "myDouble2", 0.0,     true),
                    new(@double, "myDouble3", -15.0,   true),
                    new(@double, "myDouble4", 3.14,    true),
                    new(@double, "myDouble5", 0.0,     false),
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
