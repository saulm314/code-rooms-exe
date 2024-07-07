using CREInterpreter;
using CREInterpreter.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CREInterpreterUnitTests;

public class TokenSeparatorTests
{
    [Fact]
    public void GetTokens_Null_ThrowsNullReferenceException()
    {
        string input = null!;

        Action actual = () => TokenSeparator.GetTokens(input).ToArray();

        Assert.Throws<NullReferenceException>(actual);
    }

    [Fact]
    public void GetTokens_EmptyString_ReturnsEmptyEnumerable()
    {
        string input = string.Empty;

        IEnumerable<IToken> tokens = TokenSeparator.GetTokens(input);

        Assert.Empty(tokens);
    }

    [Theory]
    [InlineData("//", "//", 0)]
    [InlineData("// ", "// ", 0)]
    [InlineData(" // ", "// ", 1)]
    [InlineData("// hello", "// hello", 0)]
    [InlineData("// hello ", "// hello ", 0)]
    [InlineData("//hello", "//hello", 0)]
    [InlineData("//hello ", "//hello ", 0)]
    [InlineData("//hello \n", "//hello ", 0)]
    [InlineData("//hello \n\n", "//hello ", 0)]
    public void GetTokens_SingleLineComment_ReturnsSingleLineCommentToken(string input, string expectedText, int expectedIndex)
    {
        IEnumerable<IToken> tokens = TokenSeparator.GetTokens(input);
        IToken token = tokens.First();
        int actualLineNumber = token.LineNumber;
        int actualIndex = token.Index;
        string actualText = token.Text;
        
        Assert.Single(tokens);
        Assert.IsAssignableFrom<SingleLineCommentToken>(token);
        Assert.Equal(1, actualLineNumber);
        Assert.Equal(expectedIndex, actualIndex);
        Assert.Equal(expectedText, actualText);
    }

    [Theory]
    [InlineData("//\n//", 1, 2, 0, 3, "//", "//")]
    [InlineData("\n//\n//", 2, 3, 1, 4, "//", "//")]
    [InlineData("//\n\n//", 1, 3, 0, 4, "//", "//")]
    [InlineData("//hello\n//", 1, 2, 0, 8, "//hello", "//")]
    [InlineData("//hello\n//hello\n", 1, 2, 0, 8, "//hello", "//hello")]
    public void GetTokens_TwoSingleLineComments_ReturnsTwoSingleLineCommentTokens(string input, int expectedLineNumber1, int expectedLineNumber2,
        int expectedIndex1, int expectedIndex2,
        string expectedText1, string expectedText2)
    {
        IEnumerable<IToken> tokens = TokenSeparator.GetTokens(input);
        int size = tokens.Count();
        IToken firstToken = tokens.First();
        int actualLineNumber1 = firstToken.LineNumber;
        int actualIndex1 = firstToken.Index;
        string actualText1 = firstToken.Text;
        IToken secondToken = tokens.Last();
        int actualLineNumber2 = secondToken.LineNumber;
        int actualIndex2 = secondToken.Index;
        string actualText2 = secondToken.Text;

        Assert.Equal(2, size);
        Assert.IsAssignableFrom<SingleLineCommentToken>(firstToken);
        Assert.IsAssignableFrom<SingleLineCommentToken>(secondToken);
        Assert.Equal(expectedLineNumber1, actualLineNumber1);
        Assert.Equal(expectedLineNumber2, actualLineNumber2);
        Assert.Equal(expectedIndex1, actualIndex1);
        Assert.Equal(expectedIndex2, actualIndex2);
        Assert.Equal(expectedText1, actualText1);
        Assert.Equal(expectedText2, actualText2);
    }

    [Theory]
    [InlineData("/**/", 1, 0, "/**/")]
    [InlineData("/* */", 1, 0, "/* */")]
    [InlineData("/* hello */", 1, 0, "/* hello */")]
    [InlineData("/* hello\n */", 1, 0, "/* hello\n */")]
    [InlineData("\n/* hello\n */\n", 2, 1, "/* hello\n */")]
    public void GetTokens_MultiLineComment_ReturnsMultiLineCommentToken(string input, int expectedLineNumber, int expectedIndex, string expectedText)
    {
        IEnumerable<IToken> tokens = TokenSeparator.GetTokens(input);
        IToken token = tokens.First();
        int actualLineNumber = token.LineNumber;
        int actualIndex = token.Index;
        string actualText = token.Text;

        Assert.Single(tokens);
        Assert.IsAssignableFrom<MultiLineCommentToken>(token);
        Assert.Equal(expectedLineNumber, actualLineNumber);
        Assert.Equal(expectedIndex, actualIndex);
        Assert.Equal(expectedText, actualText);
    }

    [Theory]
    [InlineData("/*", 1, 0, "/*")]
    [InlineData("/* ", 1, 0, "/* ")]
    [InlineData("/* \n", 1, 0, "/* \n")]
    [InlineData("/* \n\n", 1, 0, "/* \n\n")]
    [InlineData("/* hello\n\n", 1, 0, "/* hello\n\n")]
    [InlineData("/* hello\n\nhello", 1, 0, "/* hello\n\nhello")]
    [InlineData("\n/* hello\n\nhello", 2, 1, "/* hello\n\nhello")]
    [InlineData("\n\n/* hello\n\nhello", 3, 2, "/* hello\n\nhello")]
    public void GetTokens_UnclosedMultiLineComment_ReturnsInvalidToken(string input, int expectedLineNumber, int expectedIndex, string expectedText)
    {
        IEnumerable<IToken> tokens = TokenSeparator.GetTokens(input);
        IToken token = tokens.First();
        int actualLineNumber = token.LineNumber;
        int actualIndex = token.Index;
        string actualText = token.Text;

        Assert.Single(tokens);
        Assert.IsAssignableFrom<InvalidToken>(token);
        Assert.Equal(expectedLineNumber, actualLineNumber);
        Assert.Equal(expectedIndex, actualIndex);
        Assert.Equal(expectedText, actualText);
    }

    [Theory]
    [InlineData("///**/", 1, 0, "///**/")]
    [InlineData("// /* */ ", 1, 0, "// /* */ ")]
    [InlineData("// /* ", 1, 0, "// /* ")]
    public void GetTokens_SingleLineCommentContainsMultiLineComment_ReturnsSingleLineComment(string input, int expectedLineNumber, int expectedIndex,
        string expectedText)
    {
        IEnumerable<IToken> tokens = TokenSeparator.GetTokens(input);
        IToken token = tokens.First();
        int actualLineNumber = token.LineNumber;
        int actualIndex = token.Index;
        string actualText = token.Text;

        Assert.Single(tokens);
        Assert.IsAssignableFrom<SingleLineCommentToken>(token);
        Assert.Equal(expectedLineNumber, actualLineNumber);
        Assert.Equal(expectedIndex, actualIndex);
        Assert.Equal(expectedText, actualText);
    }

    [Theory]
    [InlineData("/*//*/", 1, 0, "/*//*/")]
    [InlineData("/* // */", 1, 0, "/* // */")]
    [InlineData("/* \n // \n */", 1, 0, "/* \n // \n */")]
    public void GetTokens_MultiLineCommentContainsSingleLineComment_ReturnsMultiLineComment(string input, int expectedLineNumber, int expectedIndex,
        string expectedText)
    {
        IEnumerable<IToken> tokens = TokenSeparator.GetTokens(input);
        IToken token = tokens.First();
        int actualLineNumber = token.LineNumber;
        int actualIndex = token.Index;
        string actualText = token.Text;

        Assert.Single(tokens);
        Assert.IsAssignableFrom<MultiLineCommentToken>(token);
        Assert.Equal(expectedLineNumber, actualLineNumber);
        Assert.Equal(expectedIndex, actualIndex);
        Assert.Equal(expectedText, actualText);
    }

    [Theory]
    [InlineData("true", "true", 1, 0, true)]
    [InlineData("false", "false", 1, 0, false)]
    public void GetTokens_BooleanLiteral_ReturnsBooleanLiteralToken(string input, string expectedText, int expectedLineNumber, int expectedIndex,
        bool expectedValue)
    {
        IEnumerable<IToken> tokens = TokenSeparator.GetTokens(input);
        IToken token = tokens.First();
        BooleanLiteralToken booleanLiteralToken = (BooleanLiteralToken)token;
        string actualText = token.Text;
        int actualLineNumber = token.LineNumber;
        int actualIndex = token.Index;
        bool actualValue = booleanLiteralToken.Value;

        Assert.Single(tokens);
        Assert.IsAssignableFrom<BooleanLiteralToken>(token);
        Assert.Equal(expectedText, actualText);
        Assert.Equal(expectedLineNumber, actualLineNumber);
        Assert.Equal(expectedIndex, actualIndex);
        Assert.Equal(expectedValue, actualValue);
    }

    [Theory]
    [InlineData("0", "0", 1, 0, 0)]
    [InlineData("1", "1", 1, 0, 1)]
    [InlineData("2147483647", "2147483647", 1, 0, 2147483647)]
    [InlineData("000", "000", 1, 0, 0)]
    [InlineData("001", "001", 1, 0, 1)]
    [InlineData("010", "010", 1, 0, 10)]
    public void GetTokens_IntegerLiteral_ReturnsIntegerLiteralToken(string input, string expectedText, int expectedLineNumber, int expectedIndex,
        int expectedValue)
    {
        IEnumerable<IToken> tokens = TokenSeparator.GetTokens(input);
        IToken token = tokens.First();
        IntegerLiteralToken integerLiteralToken = (IntegerLiteralToken)token;
        string actualText = token.Text;
        int actualLineNumber = token.LineNumber;
        int actualIndex = token.Index;
        int actualValue = integerLiteralToken.Value;

        Assert.Single(tokens);
        Assert.IsAssignableFrom<IntegerLiteralToken>(token);
        Assert.Equal(expectedText, actualText);
        Assert.Equal(expectedLineNumber, actualLineNumber);
        Assert.Equal(expectedIndex, actualIndex);
        Assert.Equal(expectedValue, actualValue);
    }

    [Theory]
    [InlineData("0.0", "0.0", 1, 0, 0.0)]
    [InlineData("0.5", "0.5", 1, 0, 0.5)]
    [InlineData("1.0", "1.0", 1, 0, 1.0)]
    [InlineData("1.5", "1.5", 1, 0, 1.5)]
    [InlineData("01.50", "01.50", 1, 0, 1.5)]
    public void GetTokens_DoubleFloatLiteral_ReturnsDoubleFloatLiteralToken(string input, string expectedText, int expectedLineNumber, int expectedIndex,
        double expectedValue)
    {
        IEnumerable<IToken> tokens = TokenSeparator.GetTokens(input);
        IToken token = tokens.First();
        DoubleFloatLiteralToken doubleFloatLiteralToken = (DoubleFloatLiteralToken)token;
        string actualText = token.Text;
        int actualLineNumber = token.LineNumber;
        int actualIndex = token.Index;
        double actualValue = doubleFloatLiteralToken.Value;

        Assert.Single(tokens);
        Assert.IsAssignableFrom<DoubleFloatLiteralToken>(token);
        Assert.Equal(expectedText, actualText);
        Assert.Equal(expectedLineNumber, actualLineNumber);
        Assert.Equal(expectedIndex, actualIndex);
        Assert.Equal(expectedValue, actualValue);
    }

    [Theory]
    [InlineData("0.0.0", 1, 0, "0.0.0")]
    [InlineData("0.1.0", 1, 0, "0.1.0")]
    [InlineData("0.1.1", 1, 0, "0.1.1")]
    [InlineData("1.1.1", 1, 0, "1.1.1")]
    [InlineData("1.1.0", 1, 0, "1.1.0")]
    public void GetTokens_TwoDecimalPoints_ReturnsInvalidToken(string input, int expectedLineNumber, int expectedIndex, string expectedText)
    {
        IEnumerable<IToken> tokens = TokenSeparator.GetTokens(input);
        IToken token = tokens.First();
        int actualLineNumber = token.LineNumber;
        int actualIndex = token.Index;
        string actualText = token.Text;

        Assert.Single(tokens);
        Assert.IsAssignableFrom<InvalidToken>(token);
        Assert.Equal(expectedLineNumber, actualLineNumber);
        Assert.Equal(expectedIndex, actualIndex);
        Assert.Equal(expectedText, actualText);
    }

    [Theory]
    [InlineData(@"'\0'", @"'\0'", 1, 0, '\0')]
    [InlineData(@"'\''", @"'\''", 1, 0, '\'')]
    [InlineData("'\\\"\'", "'\\\"\'", 1, 0, '"')]
    [InlineData(@"'\\'", @"'\\'", 1, 0, '\\')]
    [InlineData(@"'\a'", @"'\a'", 1, 0, '\a')]
    [InlineData(@"'\b'", @"'\b'", 1, 0, '\b')]
    [InlineData(@"'\f'", @"'\f'", 1, 0, '\f')]
    [InlineData(@"'\n'", @"'\n'", 1, 0, '\n')]
    [InlineData(@"'\r'", @"'\r'", 1, 0, '\r')]
    [InlineData(@"'\t'", @"'\t'", 1, 0, '\t')]
    [InlineData(@"'\v'", @"'\v'", 1, 0, '\v')]
    [InlineData("'a'", "'a'", 1, 0, 'a')]
    [InlineData("'3'", "'3'", 1, 0, '3')]
    [InlineData("' '", "' '", 1, 0, ' ')]
    [InlineData("'\"'", "'\"'", 1, 0, '"')]
    [InlineData("';'", "';'", 1, 0, ';')]
    [InlineData("'{'", "'{'", 1, 0, '{')]
    [InlineData("'}'", "'}'", 1, 0, '}')]
    [InlineData("'('", "'('", 1, 0, '(')]
    [InlineData("')'", "')'", 1, 0, ')')]
    [InlineData("'['", "'['", 1, 0, '[')]
    [InlineData("']'", "']'", 1, 0, ']')]
    [InlineData(@"'\q'", @"'\q'", 1, 0, 'q')]
    public void GetTokens_CharacterLiteral_ReturnsCharacterLiteralToken(string input, string expectedText, int expectedLineNumber, int expectedIndex,
        char expectedValue)
    {
        IEnumerable<IToken> tokens = TokenSeparator.GetTokens(input);
        IToken token = tokens.First();
        CharacterLiteralToken characterLiteralToken = (CharacterLiteralToken)token;
        string actualText = token.Text;
        int actualLineNumber = token.LineNumber;
        int actualIndex = token.Index;
        char actualValue = characterLiteralToken.Value;

        Assert.Single(tokens);
        Assert.IsAssignableFrom<CharacterLiteralToken>(token);
        Assert.Equal(expectedText, actualText);
        Assert.Equal(expectedLineNumber, actualLineNumber);
        Assert.Equal(expectedIndex, actualIndex);
        Assert.Equal(expectedValue, actualValue);
    }

    [Theory]
    [InlineData("'/**/a'", "'/*", 1, 0)]
    [InlineData("'\n'", "'\n'", 1, 0)]
    [InlineData("'\''", "'\''", 1, 0)]
    [InlineData("'\r'", "'\r'", 1, 0)]
    [InlineData("'\t'", "'\t'", 1, 0)]
    [InlineData("'\v'", "'\v'", 1, 0)]
    [InlineData("'\na'", "'\na'", 1, 0)]
    [InlineData("'a", "'a", 1, 0)]
    [InlineData("'\\'", "'\\'", 1, 0)]
    [InlineData("'//\na'", "'//", 1, 0)]
    [InlineData("'aa'", "'aa'", 1, 0)]
    [InlineData("'\\\n'", "'\\\n'", 1, 0)]
    [InlineData ("'\\\r'", "'\\\r'", 1, 0)]
    [InlineData("'\\\t'", "'\\\t'", 1, 0)]
    [InlineData("'\\\v'", "'\\\v'", 1, 0)]
    public void GetTokens_InvalidCharacter_ReturnsInvalidToken(string input, string expectedText, int expectedLineNumber, int expectedIndex)
    {
        IEnumerable<IToken> tokens = TokenSeparator.GetTokens(input);
        IToken token = tokens.First();
        string actualText = token.Text;
        int actualLineNumber = token.LineNumber;
        int actualIndex = token.Index;

        Assert.IsAssignableFrom<InvalidToken>(token);
        Assert.Equal(expectedText, actualText);
        Assert.Equal(expectedLineNumber, actualLineNumber);
        Assert.Equal(expectedIndex, actualIndex);
    }

    [Theory]
    [InlineData("\"\"", "\"\"", 1, 0, "")]
    [InlineData("\"a\"", "\"a\"", 1, 0, "a")]
    [InlineData("\"abc\"", "\"abc\"", 1, 0, "abc")]
    [InlineData("\"\\a\\b\\c\"", "\"\\a\\b\\c\"", 1, 0, "\\a\\b\\c")]
    [InlineData("\"';{([[)}]\\n // /* \"", "\"';{([[)}]\\n // /* \"", 1, 0, "';{([[)}]\\n // /* ")]
    public void GetTokens_StringLiteral_ReturnsStringLiteralToken(string input, string expectedText, int expectedLineNumber, int expectedIndex,
        string expectedValue)
    {
        IEnumerable<IToken> tokens = TokenSeparator.GetTokens(input);
        IToken token = tokens.First();
        StringLiteralToken stringLiteralToken = (StringLiteralToken)token;
        string actualText = token.Text;
        int actualLineNumber = token.LineNumber;
        int actualIndex = token.Index;
        string actualValue = stringLiteralToken.Value;

        Assert.Single(tokens);
        Assert.IsAssignableFrom<StringLiteralToken>(token);
        Assert.Equal(expectedText, actualText);
        Assert.Equal(expectedLineNumber, actualLineNumber);
        Assert.Equal(expectedIndex, actualIndex);
        Assert.Equal(expectedValue, actualValue);
    }

    [Theory]
    [InlineData("\"", "\"", 1, 0)]
    [InlineData("\"a", "\"a", 1, 0)]
    [InlineData("\"\n\"", "\"", 1, 0)]
    public void GetTokens_InvalidStringLiteral_ReturnsInvalidToken(string input, string expectedText, int expectedLineNumber, int expectedIndex)
    {
        IEnumerable<IToken> tokens = TokenSeparator.GetTokens(input);
        IToken token = tokens.First();
        string actualText = token.Text;
        int actualLineNumber = token.LineNumber;
        int actualIndex = token.Index;

        Assert.IsAssignableFrom<InvalidToken>(token);
        Assert.Equal(expectedText, actualText);
        Assert.Equal(expectedLineNumber, actualLineNumber);
        Assert.Equal(expectedIndex, actualIndex);
    }

    [Theory]
    [InlineData("<=", "<=", 1, 0, typeof(LessThanOrEqualToSymbolToken))]
    [InlineData(">=", ">=", 1, 0, typeof(GreaterThanOrEqualToSymbolToken))]
    [InlineData("&&", "&&", 1, 0, typeof(ConditionalAndSymbolToken))]
    [InlineData("||", "||", 1, 0, typeof(ConditionalOrSymbolToken))]
    [InlineData("==", "==", 1, 0, typeof(EqualsEqualsSymbolToken))]
    [InlineData("!=", "!=", 1, 0, typeof(NotEqualsSymbolToken))]
    [InlineData("=", "=", 1, 0, typeof(EqualsSymbolToken))]
    [InlineData("{", "{", 1, 0, typeof(OpenCurlyBraceSymbolToken))]
    [InlineData("}", "}", 1, 0, typeof(CloseCurlyBraceSymbolToken))]
    [InlineData(",", ",", 1, 0, typeof(CommaSymbolToken))]
    [InlineData(".", ".", 1, 0, typeof(DotSymbolToken))]
    [InlineData("+", "+", 1, 0, typeof(PlusSymbolToken))]
    [InlineData("-", "-", 1, 0, typeof(MinusSymbolToken))]
    [InlineData("*", "*", 1, 0, typeof(MultiplySymbolToken))]
    [InlineData("/", "/", 1, 0, typeof(DivideSymbolToken))]
    [InlineData("<", "<", 1, 0, typeof(LessThanSymbolToken))]
    [InlineData(">", ">", 1, 0, typeof(GreaterThanSymbolToken))]
    [InlineData("(", "(", 1, 0, typeof(OpenBracketSymbolToken))]
    [InlineData(")", ")", 1, 0, typeof(CloseBracketSymbolToken))]
    [InlineData("!", "!", 1, 0, typeof(NotSymbolToken))]
    [InlineData("&", "&", 1, 0, typeof(AndSymbolToken))]
    [InlineData("|", "|", 1, 0, typeof(OrSymbolToken))]
    [InlineData("^", "^", 1, 0, typeof(XorSymbolToken))]
    [InlineData("%", "%", 1, 0, typeof(RemainderSymbolToken))]
    [InlineData(";", ";", 1, 0, typeof(SemicolonSymbolToken))]
    [InlineData("[", "[", 1, 0, typeof(OpenSquareBraceSymbolToken))]
    [InlineData("]", "]", 1, 0, typeof(CloseSquareBraceSymbolToken))]
    public void GetTokens_Symbol_ReturnsSymbolToken(string input, string expectedText, int expectedLineNumber, int expectedIndex, Type expectedTokenType)
    {
        IEnumerable<IToken> tokens = TokenSeparator.GetTokens(input);
        IToken token = tokens.First();
        string actualText = token.Text;
        int actualLineNumber = token.LineNumber;
        int actualIndex = token.Index;

        Assert.Single(tokens);
        Assert.IsAssignableFrom(expectedTokenType, token);
        Assert.IsAssignableFrom<ISymbol>(token);
        Assert.Equal(expectedText, actualText);
        Assert.Equal(expectedLineNumber, actualLineNumber);
        Assert.Equal(expectedIndex, actualIndex);
    }

    [Theory]
    [InlineData("new", "new", 1, 0, typeof(NewKeywordToken))]
    [InlineData("null", "null", 1, 0, typeof(NullKeywordToken))]
    [InlineData("if", "if", 1, 0, typeof(IfKeywordToken))]
    [InlineData("else", "else", 1, 0, typeof(ElseKeywordToken))]
    [InlineData("while", "while", 1, 0, typeof(WhileKeywordToken))]
    [InlineData("for", "for", 1, 0, typeof(ForKeywordToken))]
    [InlineData("break", "break", 1, 0, typeof(BreakKeywordToken))]
    [InlineData("continue", "continue", 1, 0, typeof(ContinueKeywordToken))]
    [InlineData("Length", "Length", 1, 0, typeof(LengthKeywordToken))]
    public void GetTokens_Keyword_ReturnsKeywordToken(string input, string expectedText, int expectedLineNumber, int expectedIndex, Type expectedTokenType)
    {
        IEnumerable<IToken> tokens = TokenSeparator.GetTokens(input);
        IToken token = tokens.First();
        string actualText = token.Text;
        int actualLineNumber = token.LineNumber;
        int actualIndex = token.Index;

        Assert.Single(tokens);
        Assert.IsAssignableFrom(expectedTokenType, token);
        Assert.IsAssignableFrom<IKeyword>(token);
        Assert.Equal(expectedText, actualText);
        Assert.Equal(expectedLineNumber, actualLineNumber);
        Assert.Equal(expectedIndex, actualIndex);
    }

    [Theory]
    [InlineData("int", "int", 1, 0, typeof(int))]
    [InlineData("double", "double", 1, 0, typeof(double))]
    [InlineData("bool", "bool", 1, 0, typeof(bool))]
    [InlineData("char", "char", 1, 0, typeof(char))]
    [InlineData("string", "string", 1, 0, typeof(string))]
    public void GetTokens_TypeName_ReturnsTypeNameToken(string input, string expectedText, int expectedLineNumber, int expectedIndex, Type expectedType)
    {
        IEnumerable<IToken> tokens = TokenSeparator.GetTokens(input);
        IToken token = tokens.First();
        TypeNameToken typeNameToken = (TypeNameToken)token;
        string actualText = token.Text;
        int actualLineNumber = token.LineNumber;
        int actualIndex = token.Index;
        Type actualType = typeNameToken._VarType.SystemType;

        Assert.Single(tokens);
        Assert.IsAssignableFrom<TypeNameToken>(token);
        Assert.Equal(expectedText, actualText);
        Assert.Equal(expectedLineNumber, actualLineNumber);
        Assert.Equal(expectedIndex, actualIndex);
        Assert.Equal(expectedType, actualType);
    }

    [Theory]
    [InlineData("int[]", "int", 1, 0, typeof(int))]
    [InlineData("double[]", "double", 1, 0, typeof(double))]
    [InlineData("bool[]", "bool", 1, 0, typeof(bool))]
    [InlineData("char[]", "char", 1, 0, typeof(char))]
    [InlineData("string[]", "string", 1, 0, typeof(string))]
    public void GetTokens_ArrayTypeName_ReturnsTypeNameTokenAndSquareBraceTokens(string input, string expectedText, int expectedLineNumber, int expectedIndex,
        Type expectedType)
    {
        IToken[] tokens = TokenSeparator.GetTokens(input).ToArray();
        TypeNameToken firstToken = (TypeNameToken)tokens[0];
        IToken secondToken = tokens[1];
        IToken thirdToken = tokens[2];
        string actualText = firstToken.Text;
        int actualLineNumber = firstToken.LineNumber;
        int actualIndex = firstToken.Index;
        Type actualType = firstToken._VarType.SystemType;

        Assert.Equal(3, tokens.Length);
        Assert.IsAssignableFrom<TypeNameToken>(firstToken);
        Assert.IsAssignableFrom<OpenSquareBraceSymbolToken>(secondToken);
        Assert.IsAssignableFrom<CloseSquareBraceSymbolToken>(thirdToken);
        Assert.Equal(expectedText, actualText);
        Assert.Equal(expectedLineNumber, actualLineNumber);
        Assert.Equal(expectedIndex, actualIndex);
        Assert.Equal(expectedType, actualType);
    }

    [Theory]
    [InlineData("a", "a", 1, 0)]
    [InlineData("A", "A", 1, 0)]
    [InlineData("_", "_", 1, 0)]
    [InlineData("a0", "a0", 1, 0)]
    [InlineData("A0", "A0", 1, 0)]
    [InlineData("_0", "_0", 1, 0)]
    [InlineData("aa", "aa", 1, 0)]
    [InlineData("Aa", "Aa", 1, 0)]
    [InlineData("_a", "_a", 1, 0)]
    [InlineData("aA", "aA", 1, 0)]
    [InlineData("AA", "AA", 1, 0)]
    [InlineData("_A", "_A", 1, 0)]
    [InlineData("a_", "a_", 1, 0)]
    [InlineData("A_", "A_", 1, 0)]
    [InlineData("__", "__", 1, 0)]
    [InlineData("new_", "new_", 1, 0)]
    [InlineData("_new", "_new", 1, 0)]
    [InlineData("int_", "int_", 1, 0)]
    [InlineData("_int", "_int", 1, 0)]
    [InlineData("New", "New", 1, 0)]
    [InlineData("Int", "Int", 1, 0)]
    public void GetTokens_VariableName_ReturnsVariableNameToken(string input, string expectedText, int expectedLineNumber, int expectedIndex)
    {
        IEnumerable<IToken> tokens = TokenSeparator.GetTokens(input);
        IToken token = tokens.First();
        string actualText = token.Text;
        int actualLineNumber = token.LineNumber;
        int actualIndex = token.Index;

        Assert.Single(tokens);
        Assert.IsAssignableFrom<VariableNameToken>(token);
        Assert.Equal(expectedText, actualText);
        Assert.Equal(expectedLineNumber, actualLineNumber);
        Assert.Equal(expectedIndex, actualIndex);
    }

    [Theory]
    [InlineData("0a", "0", "a", 0, 1, 1, 0, 1)]
    [InlineData("0_", "0", "_", 0, 1, 1, 0, 1)]
    [InlineData("1a", "1", "a", 1, 1, 1, 0, 1)]
    [InlineData("1_", "1", "_", 1, 1, 1, 0, 1)]
    public void GetTokens_VariableNameStartsWithNumber_ReturnsTwoTokens(string input, string expectedText1, string expectedText2,
        int expectedValue1,
        int expectedLineNumber1, int expectedLineNumber2,
        int expectedIndex1, int expectedIndex2)
    {
        IToken[] tokens = TokenSeparator.GetTokens(input).ToArray();
        IntegerLiteralToken token1 = (IntegerLiteralToken)tokens[0];
        IToken token2 = tokens[1];
        string actualText1 = token1.Text;
        string actualText2 = token2.Text;
        int actualValue1 = token1.Value;
        int actualLineNumber1 = token1.LineNumber;
        int actualLineNumber2 = token2.LineNumber;
        int actualIndex1 = token1.Index;
        int actualIndex2 = token2.Index;

        Assert.Equal(2, tokens.Length);
        Assert.IsAssignableFrom<IntegerLiteralToken>(token1);
        Assert.IsAssignableFrom<VariableNameToken>(token2);
        Assert.Equal(expectedText1, actualText1);
        Assert.Equal(expectedText2, actualText2);
        Assert.Equal(expectedValue1, actualValue1);
        Assert.Equal(expectedLineNumber1, actualLineNumber1);
        Assert.Equal(expectedLineNumber2, actualLineNumber2);
        Assert.Equal(expectedIndex1, actualIndex1);
        Assert.Equal(expectedIndex2, actualIndex2);
    }
}