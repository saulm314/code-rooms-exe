using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests
{
    public class BadVarInitialisationWrongType : ITest
    {
        public string Path => @"Variables\BadVarInitialisationWrongType";

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
