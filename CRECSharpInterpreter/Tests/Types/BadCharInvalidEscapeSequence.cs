using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.Types
{
    public class BadCharInvalidEscapeSequence : ITest
    {
        public string Path => @"Types\BadCharInvalidEscapeSequence";

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
