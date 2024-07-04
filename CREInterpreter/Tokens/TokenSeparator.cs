using System.Collections.Generic;

namespace CREInterpreter.Tokens;

public static class TokenSeparator
{
    public static IEnumerable<IToken> GetTokens(string text)
    {
        int index = 0;
        int lineNumber = 1;
        while (index < text.Length)
        {
            yield break;
        }
    }

    private static IToken? GetWhiteSpace(string text, ref int index, ref int lineNumber)
    {
        if (text[index] == '\n')
        {
            index++;
            lineNumber++;
            return GetWhiteSpace(text, ref index, ref lineNumber);
        }
        if (char.IsWhiteSpace(text[index]))
        {
            index++;
            return GetWhiteSpace(text, ref index, ref lineNumber);
        }
        return null;
    }

    private static SingleLineCommentToken? GetSingleLineCommentToken(string text, ref int index, ref int lineNumber)
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
        return new(text[startIndex..index], originalLineNumber);
    }

    private static MultiLineCommentToken? GetMultiLineCommentToken(string text, ref int index, ref int lineNumber)
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
            throw new InterpreterException($"Multi-line comment starting at line {lineNumber} is never closed");
        index = i;
        lineNumber = lineNumberTemp;
        return new(text[startIndex..index], originalLineNumber);
    }
}