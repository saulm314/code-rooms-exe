using CREInterpreter.Tokens;
using System;
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
}