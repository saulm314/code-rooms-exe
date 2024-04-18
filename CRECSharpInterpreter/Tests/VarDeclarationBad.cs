using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests
{
    public class VarDeclarationBad : ITest
    {
        public string Path => @"VarDeclarationInit\VarDeclarationBad";

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
