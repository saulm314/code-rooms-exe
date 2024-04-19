using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.ValueTypes
{
    public class Bool : ITest
    {
        public string Path => @"ValueTypes\Bool";

        public Variable[][] Stack =>
            new[]
            {
                new Variable[]
                {
                    new(@bool, "myBool",  false,  true),
                    new(@bool, "myBool2", true,   true),
                    new(@bool, "myBool3", false,  false),
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
