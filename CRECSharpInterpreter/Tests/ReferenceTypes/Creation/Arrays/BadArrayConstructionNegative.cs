using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.ReferenceTypes.Creation.Arrays
{
    public class BadArrayConstructionNegative : ITest
    {
        public string Path => @"ReferenceTypes\Creation\Arrays\BadArrayConstructionNegative";

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
