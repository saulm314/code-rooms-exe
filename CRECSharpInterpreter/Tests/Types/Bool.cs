using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.Types
{
    public class Bool : ITest
    {
        public string Path => @"Types\Bool";

        public Variable[][] Stack =>
            new[]
            {
                new Variable[]
                {
                    new(@bool, "myBool",  false,  true),
                    new(@bool, "myBool2", true,   true),
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
