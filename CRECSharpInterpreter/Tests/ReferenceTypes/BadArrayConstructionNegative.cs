using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.ReferenceTypes
{
    public class BadArrayConstructionNegative : ITest
    {
        public string Path => @"ReferenceTypes\BadArrayConstructionNegative";

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
