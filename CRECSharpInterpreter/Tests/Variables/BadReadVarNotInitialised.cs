using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.Variables
{
    public class BadReadVarNotInitialised : ITest
    {
        public string Path => @"Variables\BadReadVarNotInitialised";

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
