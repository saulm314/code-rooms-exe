using CRECSharpInterpreter.Tests.Variables;
using CRECSharpInterpreter.Tests.Types;

namespace CRECSharpInterpreter.Tests
{
    public static class TestRepository
    {
        public static ITest[] Tests { get; } = new ITest[]
        {
            new VarDeclaration(),
            new BadVarDeclarationType(),
            new BadVarDeclarationKeyword(),
            new VarInitialisation(),
            new BadVarInitialisationWrongType(),
            new ReadWriteVar(),
            new BadReadVarNotDeclared(),
            new BadReadVarNotInitialised(),
            new Int(),
            new BadIntDouble(),
            new BadIntOutOfBounds(),
            new Bool(),
            new Double(),
            new BadDoubleInt()
        };

        public static int SuccessfulTests { get; set; } = 0;
        public static int TotalTests { get; set; } = 0;
    }
}
