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

    [Fact]
    public void GetTokens_Semicolon_ReturnsSemicolonSymbolToken()
    {
        string input = ";";

        IEnumerable<IToken> tokens = TokenSeparator.GetTokens(input);
        IToken token = tokens.First();
        int actualLineNumber = token.LineNumber;
        
        Assert.Single(tokens);
        Assert.IsAssignableFrom<SemicolonSymbolToken>(token);
        Assert.Equal(1, actualLineNumber);
    }
}