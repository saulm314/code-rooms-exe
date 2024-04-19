using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.Types
{
    public class BadIntDouble : ITest
    {
        public string Path => @"Types\BadIntDouble";

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
