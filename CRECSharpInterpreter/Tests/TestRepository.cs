namespace CRECSharpInterpreter.Tests
{
    public static class TestRepository
    {
        public static ITest[] Tests { get; } = new ITest[]
        {
            new VarDeclaration(),
            new BadVarDeclarationType(),
            new BadVarDeclarationKeyword(),
            new VarInitialisation()
        };

        public static int SuccessfulTests { get; set; } = 0;
        public static int TotalTests { get; set; } = 0;
    }
}
