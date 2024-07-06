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
    [InlineData("//", "//")]
    [InlineData("// ", "// ")]
    [InlineData(" // ", "// ")]
    [InlineData("// hello", "// hello")]
    [InlineData("// hello ", "// hello ")]
    [InlineData("//hello", "//hello")]
    [InlineData("//hello ", "//hello ")]
    [InlineData("//hello \n", "//hello ")]
    [InlineData("//hello \n\n", "//hello ")]
    public void GetTokens_SingleLineComment_ReturnsSingleLineCommentToken(string input, string expectedText)
    {
        IEnumerable<IToken> tokens = TokenSeparator.GetTokens(input);
        IToken token = tokens.First();
        int actualLineNumber = token.LineNumber;
        string actualText = token.Text;
        
        Assert.Single(tokens);
        Assert.IsAssignableFrom<SingleLineCommentToken>(token);
        Assert.Equal(1, actualLineNumber);
        Assert.Equal(expectedText, actualText);
    }

    [Theory]
    [InlineData("//\n//", 1, 2, "//", "//")]
    [InlineData("\n//\n//", 2, 3, "//", "//")]
    [InlineData("//\n\n//", 1, 3, "//", "//")]
    [InlineData("//hello\n//", 1, 2, "//hello", "//")]
    [InlineData("//hello\n//hello\n", 1, 2, "//hello", "//hello")]
    public void GetTokens_TwoSingleLineComments_ReturnsTwoSingleLineCommentTokens(string input, int expectedLineNumber1, int expectedLineNumber2,
        string expectedText1, string expectedText2)
    {
        IEnumerable<IToken> tokens = TokenSeparator.GetTokens(input);
        int size = tokens.Count();
        IToken firstToken = tokens.First();
        int actualLineNumber1 = firstToken.LineNumber;
        string actualText1 = firstToken.Text;
        IToken secondToken = tokens.Last();
        int actualLineNumber2 = secondToken.LineNumber;
        string actualText2 = secondToken.Text;

        Assert.Equal(2, size);
        Assert.IsAssignableFrom<SingleLineCommentToken>(firstToken);
        Assert.IsAssignableFrom<SingleLineCommentToken>(secondToken);
        Assert.Equal(expectedLineNumber1, actualLineNumber1);
        Assert.Equal(expectedLineNumber2, actualLineNumber2);
        Assert.Equal(expectedText1, actualText1);
        Assert.Equal(expectedText2, actualText2);
    }

    [Theory]
    [InlineData("/**/", 1, "/**/")]
    [InlineData("/* */", 1, "/* */")]
    [InlineData("/* hello */", 1, "/* hello */")]
    [InlineData("/* hello\n */", 1, "/* hello\n */")]
    [InlineData("\n/* hello\n */\n", 2, "/* hello\n */")]
    public void GetTokens_MultiLineComment_ReturnsMultiLineCommentToken(string input, int expectedLineNumber, string expectedText)
    {
        IEnumerable<IToken> tokens = TokenSeparator.GetTokens(input);
        IToken token = tokens.First();
        int actualLineNumber = token.LineNumber;
        string actualText = token.Text;

        Assert.Single(tokens);
        Assert.IsAssignableFrom<MultiLineCommentToken>(token);
        Assert.Equal(expectedLineNumber, actualLineNumber);
        Assert.Equal(expectedText, actualText);
    }

    [Theory]
    [InlineData("/*", 1, "/*")]
    [InlineData("/* ", 1, "/* ")]
    [InlineData("/* \n", 1, "/* \n")]
    [InlineData("/* \n\n", 1, "/* \n\n")]
    [InlineData("/* hello\n\n", 1, "/* hello\n\n")]
    [InlineData("/* hello\n\nhello", 1, "/* hello\n\nhello")]
    [InlineData("\n/* hello\n\nhello", 2, "/* hello\n\nhello")]
    [InlineData("\n\n/* hello\n\nhello", 3, "/* hello\n\nhello")]
    public void GetTokens_UnclosedMultiLineComment_ReturnsInvalidToken(string input, int expectedLineNumber, string expectedText)
    {
        IEnumerable<IToken> tokens = TokenSeparator.GetTokens(input);
        IToken token = tokens.First();
        int actualLineNumber = token.LineNumber;
        string actualText = token.Text;

        Assert.Single(tokens);
        Assert.IsAssignableFrom<InvalidToken>(token);
        Assert.Equal(expectedLineNumber, actualLineNumber);
        Assert.Equal(expectedText, actualText);
    }

    [Theory]
    [InlineData("///**/", 1, "///**/")]
    [InlineData("// /* */ ", 1, "// /* */ ")]
    [InlineData("// /* ", 1, "// /* ")]
    public void GetTokens_SingleLineCommentContainsMultiLineComment_ReturnsSingleLineComment(string input, int expectedLineNumber, string expectedText)
    {
        IEnumerable<IToken> tokens = TokenSeparator.GetTokens(input);
        IToken token = tokens.First();
        int actualLineNumber = token.LineNumber;
        string actualText = token.Text;

        Assert.Single(tokens);
        Assert.IsAssignableFrom<SingleLineCommentToken>(token);
        Assert.Equal(expectedLineNumber, actualLineNumber);
        Assert.Equal(expectedText, actualText);
    }

    [Theory]
    [InlineData("/*//*/", 1, "/*//*/")]
    [InlineData("/* // */", 1, "/* // */")]
    [InlineData("/* \n // \n */", 1, "/* \n // \n */")]
    public void GetTokens_MultiLineCommentContainsSingleLineComment_ReturnsMultiLineComment(string input, int expectedLineNumber, string expectedText)
    {
        IEnumerable<IToken> tokens = TokenSeparator.GetTokens(input);
        IToken token = tokens.First();
        int actualLineNumber = token.LineNumber;
        string actualText = token.Text;

        Assert.Single(tokens);
        Assert.IsAssignableFrom<MultiLineCommentToken>(token);
        Assert.Equal(expectedLineNumber, actualLineNumber);
        Assert.Equal(expectedText, actualText);
    }

    [Theory]
    [InlineData("true", "true", 1, true)]
    [InlineData("false", "false", 1, false)]
    public void GetTokens_BooleanLiteral_ReturnsBooleanLiteralToken(string input, string expectedText, int expectedLineNumber, bool expectedValue)
    {
        IEnumerable<IToken> tokens = TokenSeparator.GetTokens(input);
        IToken token = tokens.First();
        BooleanLiteralToken booleanLiteralToken = (BooleanLiteralToken)token;
        string actualText = token.Text;
        int actualLineNumber = token.LineNumber;
        bool actualValue = booleanLiteralToken.Value;

        Assert.Single(tokens);
        Assert.IsAssignableFrom<BooleanLiteralToken>(token);
        Assert.Equal(expectedText, actualText);
        Assert.Equal(expectedLineNumber, actualLineNumber);
        Assert.Equal(expectedValue, actualValue);
    }

    [Theory]
    [InlineData("0", "0", 1, 0)]
    [InlineData("1", "1", 1, 1)]
    [InlineData("2147483647", "2147483647", 1, 2147483647)]
    [InlineData("000", "000", 1, 0)]
    [InlineData("001", "001", 1, 1)]
    [InlineData("010", "010", 1, 10)]
    public void GetTokens_IntegerLiteral_ReturnsIntegerLiteralToken(string input, string expectedText, int expectedLineNumber, int expectedValue)
    {
        IEnumerable<IToken> tokens = TokenSeparator.GetTokens(input);
        IToken token = tokens.First();
        IntegerLiteralToken integerLiteralToken = (IntegerLiteralToken)token;
        string actualText = token.Text;
        int actualLineNumber = token.LineNumber;
        int actualValue = integerLiteralToken.Value;

        Assert.Single(tokens);
        Assert.IsAssignableFrom<IntegerLiteralToken>(token);
        Assert.Equal(expectedText, actualText);
        Assert.Equal(expectedLineNumber, actualLineNumber);
        Assert.Equal(expectedValue, actualValue);
    }

    [Theory]
    [InlineData("0.0", "0.0", 1, 0.0)]
    [InlineData("0.5", "0.5", 1, 0.5)]
    [InlineData("1.0", "1.0", 1, 1.0)]
    [InlineData("1.5", "1.5", 1, 1.5)]
    [InlineData("01.50", "01.50", 1, 1.5)]
    public void GetTokens_DoubleFloatLiteral_ReturnsDoubleFloatLiteralToken(string input, string expectedText, int expectedLineNumber, double expectedValue)
    {
        IEnumerable<IToken> tokens = TokenSeparator.GetTokens(input);
        IToken token = tokens.First();
        DoubleFloatLiteralToken doubleFloatLiteralToken = (DoubleFloatLiteralToken)token;
        string actualText = token.Text;
        int actualLineNumber = token.LineNumber;
        double actualValue = doubleFloatLiteralToken.Value;

        Assert.Single(tokens);
        Assert.IsAssignableFrom<DoubleFloatLiteralToken>(token);
        Assert.Equal(expectedText, actualText);
        Assert.Equal(expectedLineNumber, actualLineNumber);
        Assert.Equal(expectedValue, actualValue);
    }

    [Theory]
    [InlineData("0.0.0", 1, "0.0.0")]
    [InlineData("0.1.0", 1, "0.1.0")]
    [InlineData("0.1.1", 1, "0.1.1")]
    [InlineData("1.1.1", 1, "1.1.1")]
    [InlineData("1.1.0", 1, "1.1.0")]
    public void GetTokens_TwoDecimalPoints_ReturnsInvalidToken(string input, int expectedLineNumber, string expectedText)
    {
        IEnumerable<IToken> tokens = TokenSeparator.GetTokens(input);
        IToken token = tokens.First();
        int actualLineNumber = token.LineNumber;
        string actualText = token.Text;

        Assert.Single(tokens);
        Assert.IsAssignableFrom<InvalidToken>(token);
        Assert.Equal(expectedLineNumber, actualLineNumber);
        Assert.Equal(expectedText, actualText);
    }

    [Theory]
    [InlineData(@"'\0'", @"'\0'", 1, '\0')]
    [InlineData(@"'\''", @"'\''", 1, '\'')]
    [InlineData("'\\\"\'", "'\\\"\'", 1, '"')]
    [InlineData(@"'\\'", @"'\\'", 1, '\\')]
    [InlineData(@"'\a'", @"'\a'", 1, '\a')]
    [InlineData(@"'\b'", @"'\b'", 1, '\b')]
    [InlineData(@"'\f'", @"'\f'", 1, '\f')]
    [InlineData(@"'\n'", @"'\n'", 1, '\n')]
    [InlineData(@"'\r'", @"'\r'", 1, '\r')]
    [InlineData(@"'\t'", @"'\t'", 1, '\t')]
    [InlineData(@"'\v'", @"'\v'", 1, '\v')]
    [InlineData("'a'", "'a'", 1, 'a')]
    [InlineData("'3'", "'3'", 1, '3')]
    [InlineData("' '", "' '", 1, ' ')]
    [InlineData("'\"'", "'\"'", 1, '"')]
    [InlineData("';'", "';'", 1, ';')]
    [InlineData("'{'", "'{'", 1, '{')]
    [InlineData("'}'", "'}'", 1, '}')]
    [InlineData("'('", "'('", 1, '(')]
    [InlineData("')'", "')'", 1, ')')]
    [InlineData("'['", "'['", 1, '[')]
    [InlineData("']'", "']'", 1, ']')]
    [InlineData(@"'\q'", @"'\q'", 1, 'q')]
    public void GetTokens_CharacterLiteral_ReturnsCharacterLiteralToken(string input, string expectedText, int expectedLineNumber, char expectedValue)
    {
        IEnumerable<IToken> tokens = TokenSeparator.GetTokens(input);
        IToken token = tokens.First();
        CharacterLiteralToken characterLiteralToken = (CharacterLiteralToken)token;
        string actualText = token.Text;
        int actualLineNumber = token.LineNumber;
        char actualValue = characterLiteralToken.Value;

        Assert.Single(tokens);
        Assert.IsAssignableFrom<CharacterLiteralToken>(token);
        Assert.Equal(expectedText, actualText);
        Assert.Equal(expectedLineNumber, actualLineNumber);
        Assert.Equal(expectedValue, actualValue);
    }

    [Theory]
    [InlineData("'/**/a'", "'/*", 1)]
    [InlineData("'\n'", "'\n'", 1)]
    [InlineData("'\''", "'\''", 1)]
    [InlineData("'\r'", "'\r'", 1)]
    [InlineData("'\t'", "'\t'", 1)]
    [InlineData("'\v'", "'\v'", 1)]
    [InlineData("'\na'", "'\na'", 1)]
    [InlineData("'a", "'a", 1)]
    [InlineData("'\\'", "'\\'", 1)]
    [InlineData("'//\na'", "'//", 1)]
    [InlineData("'aa'", "'aa'", 1)]
    [InlineData("'\\\n'", "'\\\n'", 1)]
    [InlineData("'\\\r'", "'\\\r'", 1)]
    [InlineData("'\\\t'", "'\\\t'", 1)]
    [InlineData("'\\\v'", "'\\\v'", 1)]
    public void GetTokens_InvalidCharacter_ReturnsInvalidToken(string input, string expectedText, int expectedLineNumber)
    {
        IEnumerable<IToken> tokens = TokenSeparator.GetTokens(input);
        IToken token = tokens.First();
        string actualText = token.Text;
        int actualLineNumber = token.LineNumber;

        Assert.IsAssignableFrom<InvalidToken>(token);
        Assert.Equal(expectedText, actualText);
        Assert.Equal(expectedLineNumber, actualLineNumber);
    }

    [Theory]
    [InlineData("\"\"", "\"\"", 1, "")]
    [InlineData("\"a\"", "\"a\"", 1, "a")]
    [InlineData("\"abc\"", "\"abc\"", 1, "abc")]
    [InlineData("\"\\a\\b\\c\"", "\"\\a\\b\\c\"", 1, "\\a\\b\\c")]
    [InlineData("\"';{([[)}]\\n // /* \"", "\"';{([[)}]\\n // /* \"", 1, "';{([[)}]\\n // /* ")]
    public void GetTokens_StringLiteral_ReturnsStringLiteralToken(string input, string expectedText, int expectedLineNumber, string expectedValue)
    {
        IEnumerable<IToken> tokens = TokenSeparator.GetTokens(input);
        IToken token = tokens.First();
        StringLiteralToken stringLiteralToken = (StringLiteralToken)token;
        string actualText = token.Text;
        int actualLineNumber = token.LineNumber;
        string actualValue = stringLiteralToken.Value;

        Assert.Single(tokens);
        Assert.IsAssignableFrom<StringLiteralToken>(token);
        Assert.Equal(expectedText, actualText);
        Assert.Equal(expectedLineNumber, actualLineNumber);
        Assert.Equal(expectedValue, actualValue);
    }

    [Theory]
    [InlineData("\"", "\"", 1)]
    [InlineData("\"a", "\"a", 1)]
    [InlineData("\"\n\"", "\"", 1)]
    public void GetTokens_InvalidStringLiteral_ReturnsInvalidToken(string input, string expectedText, int expectedLineNumber)
    {
        IEnumerable<IToken> tokens = TokenSeparator.GetTokens(input);
        IToken token = tokens.First();
        string actualText = token.Text;
        int actualLineNumber = token.LineNumber;

        Assert.IsAssignableFrom<InvalidToken>(token);
        Assert.Equal(expectedText, actualText);
        Assert.Equal(expectedLineNumber, actualLineNumber);
    }

    [Theory]
    [InlineData("<=", "<=", 1, typeof(LessThanOrEqualToSymbolToken))]
    [InlineData(">=", ">=", 1, typeof(GreaterThanOrEqualToSymbolToken))]
    [InlineData("&&", "&&", 1, typeof(ConditionalAndSymbolToken))]
    [InlineData("||", "||", 1, typeof(ConditionalOrSymbolToken))]
    [InlineData("==", "==", 1, typeof(EqualsEqualsSymbolToken))]
    [InlineData("!=", "!=", 1, typeof(NotEqualsSymbolToken))]
    [InlineData("=", "=", 1, typeof(EqualsSymbolToken))]
    [InlineData("{", "{", 1, typeof(OpenCurlyBraceSymbolToken))]
    [InlineData("}", "}", 1, typeof(CloseCurlyBraceSymbolToken))]
    [InlineData(",", ",", 1, typeof(CommaSymbolToken))]
    [InlineData(".", ".", 1, typeof(DotSymbolToken))]
    [InlineData("+", "+", 1, typeof(PlusSymbolToken))]
    [InlineData("-", "-", 1, typeof(MinusSymbolToken))]
    [InlineData("*", "*", 1, typeof(MultiplySymbolToken))]
    [InlineData("/", "/", 1, typeof(DivideSymbolToken))]
    [InlineData("<", "<", 1, typeof(LessThanSymbolToken))]
    [InlineData(">", ">", 1, typeof(GreaterThanSymbolToken))]
    [InlineData("(", "(", 1, typeof(OpenBracketSymbolToken))]
    [InlineData(")", ")", 1, typeof(CloseBracketSymbolToken))]
    [InlineData("!", "!", 1, typeof(NotSymbolToken))]
    [InlineData("&", "&", 1, typeof(AndSymbolToken))]
    [InlineData("|", "|", 1, typeof(OrSymbolToken))]
    [InlineData("^", "^", 1, typeof(XorSymbolToken))]
    [InlineData("%", "%", 1, typeof(RemainderSymbolToken))]
    [InlineData(";", ";", 1, typeof(SemicolonSymbolToken))]
    [InlineData("[", "[", 1, typeof(OpenSquareBraceSymbolToken))]
    [InlineData("]", "]", 1, typeof(CloseSquareBraceSymbolToken))]
    public void GetTokens_Symbol_ReturnsSymbolToken(string input, string expectedText, int expectedLineNumber, Type expectedTokenType)
    {
        IEnumerable<IToken> tokens = TokenSeparator.GetTokens(input);
        IToken token = tokens.First();
        string actualText = token.Text;
        int actualLineNumber = token.LineNumber;

        Assert.Single(tokens);
        Assert.IsAssignableFrom(expectedTokenType, token);
        Assert.IsAssignableFrom<ISymbol>(token);
        Assert.Equal(expectedText, actualText);
        Assert.Equal(expectedLineNumber, actualLineNumber);
    }

    [Theory]
    [InlineData("new", "new", 1, typeof(NewKeywordToken))]
    [InlineData("null", "null", 1, typeof(NullKeywordToken))]
    [InlineData("if", "if", 1, typeof(IfKeywordToken))]
    [InlineData("else", "else", 1, typeof(ElseKeywordToken))]
    [InlineData("while", "while", 1, typeof(WhileKeywordToken))]
    [InlineData("for", "for", 1, typeof(ForKeywordToken))]
    [InlineData("break", "break", 1, typeof(BreakKeywordToken))]
    [InlineData("continue", "continue", 1, typeof(ContinueKeywordToken))]
    [InlineData("Length", "Length", 1, typeof(LengthKeywordToken))]
    public void GetTokens_Keyword_ReturnsKeywordToken(string input, string expectedText, int expectedLineNumber, Type expectedTokenType)
    {
        IEnumerable<IToken> tokens = TokenSeparator.GetTokens(input);
        IToken token = tokens.First();
        string actualText = token.Text;
        int actualLineNumber = token.LineNumber;

        Assert.Single(tokens);
        Assert.IsAssignableFrom(expectedTokenType, token);
        Assert.IsAssignableFrom<IKeyword>(token);
        Assert.Equal(expectedText, actualText);
        Assert.Equal(expectedLineNumber, actualLineNumber);
    }

    [Theory]
    [InlineData("int", "int", 1, typeof(int))]
    [InlineData("double", "double", 1, typeof(double))]
    [InlineData("bool", "bool", 1, typeof(bool))]
    [InlineData("char", "char", 1, typeof(char))]
    [InlineData("string", "string", 1, typeof(string))]
    public void GetTokens_TypeName_ReturnsTypeNameToken(string input, string expectedText, int expectedLineNumber, Type expectedType)
    {
        IEnumerable<IToken> tokens = TokenSeparator.GetTokens(input);
        IToken token = tokens.First();
        TypeNameToken typeNameToken = (TypeNameToken)token;
        string actualText = token.Text;
        int actualLineNumber = token.LineNumber;
        Type actualType = typeNameToken._VarType.SystemType;

        Assert.Single(tokens);
        Assert.IsAssignableFrom<TypeNameToken>(token);
        Assert.Equal(expectedText, actualText);
        Assert.Equal(expectedLineNumber, actualLineNumber);
        Assert.Equal(expectedType, actualType);
    }

    [Theory]
    [InlineData("int[]", "int", 1, typeof(int))]
    [InlineData("double[]", "double", 1, typeof(double))]
    [InlineData("bool[]", "bool", 1, typeof(bool))]
    [InlineData("char[]", "char", 1, typeof(char))]
    [InlineData("string[]", "string", 1, typeof(string))]
    public void GetTokens_ArrayTypeName_ReturnsTypeNameTokenAndSquareBraceTokens(string input, string expectedText, int expectedLineNumber, Type expectedType)
    {
        IToken[] tokens = TokenSeparator.GetTokens(input).ToArray();
        TypeNameToken firstToken = (TypeNameToken)tokens[0];
        IToken secondToken = tokens[1];
        IToken thirdToken = tokens[2];
        string actualText = firstToken.Text;
        int actualLineNumber = firstToken.LineNumber;
        Type actualType = firstToken._VarType.SystemType;

        Assert.Equal(3, tokens.Length);
        Assert.IsAssignableFrom<TypeNameToken>(firstToken);
        Assert.IsAssignableFrom<OpenSquareBraceSymbolToken>(secondToken);
        Assert.IsAssignableFrom<CloseSquareBraceSymbolToken>(thirdToken);
        Assert.Equal(expectedText, actualText);
        Assert.Equal(expectedLineNumber, actualLineNumber);
        Assert.Equal(expectedType, actualType);
    }

    [Theory]
    [InlineData("a", "a", 1)]
    [InlineData("A", "A", 1)]
    [InlineData("_", "_", 1)]
    [InlineData("a0", "a0", 1)]
    [InlineData("A0", "A0", 1)]
    [InlineData("_0", "_0", 1)]
    [InlineData("aa", "aa", 1)]
    [InlineData("Aa", "Aa", 1)]
    [InlineData("_a", "_a", 1)]
    [InlineData("aA", "aA", 1)]
    [InlineData("AA", "AA", 1)]
    [InlineData("_A", "_A", 1)]
    [InlineData("a_", "a_", 1)]
    [InlineData("A_", "A_", 1)]
    [InlineData("__", "__", 1)]
    [InlineData("new_", "new_", 1)]
    [InlineData("_new", "_new", 1)]
    [InlineData("int_", "int_", 1)]
    [InlineData("_int", "_int", 1)]
    public void GetTokens_VariableName_ReturnsVariableNameToken(string input, string expectedText, int expectedLineNumber)
    {
        IEnumerable<IToken> tokens = TokenSeparator.GetTokens(input);
        IToken token = tokens.First();
        string actualText = token.Text;
        int actualLineNumber = token.LineNumber;

        Assert.Single(tokens);
        Assert.IsAssignableFrom<VariableNameToken>(token);
        Assert.Equal(expectedText, actualText);
        Assert.Equal(expectedLineNumber, actualLineNumber);
    }
}