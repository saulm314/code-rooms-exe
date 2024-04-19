using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.ReferenceTypes
{
    public class BadArrayElementReadEmptyArr : ITest
    {
        public string Path => @"ReferenceTypes\BadArrayElementReadEmptyArr";

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
