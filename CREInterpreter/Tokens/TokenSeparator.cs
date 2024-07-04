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

    private static IToken? GetSingleLineCommentToken(string text, ref int index, ref int lineNumber)
    {
        int startIndex = index;
        int i;
        int originalLineNumber = lineNumber;
        int lineNumberTemp = lineNumber;
        if (text.Length - startIndex < 2)
            return null;
        if (text[startIndex] != '/')
            return null;
        if (text[startIndex + 1] != '/')
            return null;
        i = startIndex + 2;
        while (i < text.Length && text[i] != '\n')
            i++;
        if (i < text.Length)
            lineNumberTemp++;
        index = i;
        lineNumber = lineNumberTemp;
        return new SingleLineCommentToken(text[startIndex..index], originalLineNumber);
    }

    private static IToken? GetMultiLineCommentToken(string text, ref int index, ref int lineNumber)
    {
        int startIndex = index;
        int i;
        int originalLineNumber = lineNumber;
        int lineNumberTemp = lineNumber;
        if (text.Length - startIndex < 4)
            return null;
        if (text[startIndex] != '/')
            return null;
        if (text[startIndex + 1] != '*')
            return null;
        i = startIndex + 2;
        bool previousIsAsterisk = false;
        bool closed = false;
        while (i < text.Length)
        {
            if (text[i] == '*')
            {
                previousIsAsterisk = true;
                i++;
                continue;
            }
            if (text[i] == '/' && previousIsAsterisk)
            {
                i++;
                closed = true;
                break;
            }
            if (text[i] == '\n')
                lineNumberTemp++;
            previousIsAsterisk = false;
            i++;
        }
        if (!closed)
            return null;
        index = i;
        lineNumber = lineNumberTemp;
        return new MultiLineCommentToken(text[startIndex..index], originalLineNumber);
    }
}