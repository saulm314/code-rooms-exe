using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.Types
{
    public class BadDoubleInt : ITest
    {
        public string Path => @"Types\BadDoubleInt";

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
