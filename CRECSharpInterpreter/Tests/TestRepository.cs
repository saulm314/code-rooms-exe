using System.Collections.Generic;

namespace CRECSharpInterpreter.Tests
{
    public static class TestRepository
    {
        public static ITest[] Tests { get; } = new ITest[]
        {
            new VarDeclaration()
        };
    }
}
