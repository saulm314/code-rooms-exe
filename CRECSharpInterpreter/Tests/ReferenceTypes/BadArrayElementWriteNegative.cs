using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.ReferenceTypes
{
    public class BadArrayElementWriteNegative : ITest
    {
        public string Path => @"ReferenceTypes\BadArrayElementWriteNegative";

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
