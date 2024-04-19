using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.ReferenceTypes.Indexing.Arrays
{
    public class BadArrayElementWriteNotInteger : ITest
    {
        public string Path => @"ReferenceTypes\Indexing\Arrays\BadArrayElementWriteNotInteger";

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
