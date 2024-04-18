using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests
{
    public class BadVarDeclarationType : ITest
    {
        public string Path => @"VarDeclarationInit\BadVarDeclarationType";

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
