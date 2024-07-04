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
}