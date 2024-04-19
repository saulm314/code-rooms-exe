using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.ReferenceTypes
{
    public class BadArrayElementReadNotInteger : ITest
    {
        public string Path => @"ReferenceTypes\BadArrayElementReadNotInteger";

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
