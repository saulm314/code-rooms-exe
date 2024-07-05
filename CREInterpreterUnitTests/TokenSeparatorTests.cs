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
    [InlineData("a'", "a'", 1)]
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
}