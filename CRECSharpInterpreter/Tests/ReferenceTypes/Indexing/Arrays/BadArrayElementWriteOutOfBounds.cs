using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.ReferenceTypes.Indexing.Arrays
{
    public class BadArrayElementWriteOutOfBounds : ITest
    {
        public string Path => @"ReferenceTypes\Indexing\Arrays\BadArrayElementWriteOutOfBounds";

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

        public Error Error => Error.Run;
    }
}
