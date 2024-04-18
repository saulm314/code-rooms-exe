using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.Variables
{
    public class BadVarDeclarationKeyword : ITest
    {
        public string Path => @"Variables\BadVarDeclarationKeyword";

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
