using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.ValueTypes
{
    public class BadCharTwoChars : ITest
    {
        public string Path => @"ValueTypes\BadCharTwoChars";

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
