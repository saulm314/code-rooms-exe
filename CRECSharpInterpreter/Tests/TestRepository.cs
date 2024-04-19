using CRECSharpInterpreter.Tests.Variables;
using CRECSharpInterpreter.Tests.ValueTypes;
using CRECSharpInterpreter.Tests.ReferenceTypes.Creation.Arrays;
using CRECSharpInterpreter.Tests.ReferenceTypes.Indexing.Arrays;

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
            new BadDoubleInt(),
            new Char(),
            new BadCharEmpty(),
            new BadCharInvalidEscapeSequence(),
            new BadCharNewline(),
            new BadCharNewlineChar(),
            new BadCharSingleBackslash(),
            new BadCharQuoteNotClosed(),
            new BadCharQuoteNotOpened(),
            new BadCharTwoChars(),
            new ValueTypeArrayDeclInit(),
            new ValueTypeArrayConstruction(),
            new BadArrayConstructionNegative(),
            new BadArrayConstructionNotInteger(),
            new BadArrayConstructionNoArg(),
            new ValueTypeArrayLiteral(),
            new BadArrayLiteralWrongType(),
            new ValueTypeArrayElementReadWrite(),
            new BadArrayElementReadOutOfBounds(),
            new BadArrayElementReadNegative(),
            new BadArrayElementReadEmptyArr(),
            new BadArrayElementReadNoArg(),
            new BadArrayElementReadNotInteger(),
            new BadArrayElementWriteOutOfBounds(),
            new BadArrayElementWriteNegative(),
            new BadArrayElementWriteEmptyArr(),
            new BadArrayElementWriteNoArg(),
            new BadArrayElementWriteNotInteger()
        };

        public static int SuccessfulTests { get; set; } = 0;
        public static int TotalTests { get; set; } = 0;
    }
}
