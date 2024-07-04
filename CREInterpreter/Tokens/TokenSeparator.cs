using System.Collections.Generic;
using System.Text;

namespace CREInterpreter.Tokens;

public static class TokenSeparator
{
    public static IEnumerable<IToken> GetTokens(string text)
    {
        int i = 0;
        while (i < text.Length)
        {

        }
    }

    private static IToken? GetSingleLineCommentToken(string text, int startIndex, out int endIndex, ref int lineNumber)
    {
        endIndex = startIndex;
        if (text.Length - startIndex < 2)
            return null;
        if (text[startIndex] != '/')
            return null;
        if (text[startIndex + 1] != '/')
            return null;
        StringBuilder commentBuilder = new("//");
        int i = startIndex + 2;
        while (i < text.Length && text[i] != '\n')
            commentBuilder.Append(text[i++]);
        endIndex = i;
        SingleLineCommentToken token = new(text[startIndex..endIndex], lineNumber);
        if (i < text.Length)
            lineNumber++;
        return token;
    }
}