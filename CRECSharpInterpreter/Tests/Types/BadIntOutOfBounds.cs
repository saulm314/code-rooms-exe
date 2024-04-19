using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.Types
{
    public class BadIntOutOfBounds : ITest
    {
        public string Path => @"Types\BadIntOutOfBounds";

        public Variable[][] Stack =>
            new[]
            {
                new Variable[]
                {
                }
            };

        public Variable[] Heap =>
            new Variable[]
            {
                new(null)
            };

        public Error Error => Error.Compile;
    }
}
