﻿using CRECSharpInterpreter.Tests.Variables;
using CRECSharpInterpreter.Tests.ValueTypes;
using CRECSharpInterpreter.Tests.ReferenceTypes;

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
            new BadCharTwoChars(),
            new ValueTypeArrayDeclInit(),
            new ValueTypeArrayConstruction()
        };

        public static int SuccessfulTests { get; set; } = 0;
        public static int TotalTests { get; set; } = 0;
    }
}
