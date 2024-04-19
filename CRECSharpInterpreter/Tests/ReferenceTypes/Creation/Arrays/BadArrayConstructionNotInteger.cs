using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.ReferenceTypes.Creation.Arrays
{
    public class BadArrayConstructionNotInteger : ITest
    {
        public string Path => @"ReferenceTypes\Creation\Arrays\BadArrayConstructionNotInteger";

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
