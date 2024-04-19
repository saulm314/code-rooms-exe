using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.ReferenceTypes.Indexing.Arrays
{
    public class BadArrayElementReadOutOfBounds : ITest
    {
        public string Path => @"ReferenceTypes\Indexing\Arrays\BadArrayElementReadOutOfBounds";

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
