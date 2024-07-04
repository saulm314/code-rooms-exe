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
            SkipWhiteSpace(text, ref index, ref lineNumber);
            if (index >= text.Length)
                yield break;
            yield return
                GetSingleLineCommentToken(text, ref index, ref lineNumber) ??
                GetMultiLineCommentToken(text, ref index, ref lineNumber) ?? 
                GetInvalidToken(text, ref index, ref lineNumber);
        }
    }

    private static void SkipWhiteSpace(string text, ref int index, ref int lineNumber)
    {
        if (index >= text.Length)
            return;
        if (text[index] == '\n')
        {
            index++;
            lineNumber++;
            SkipWhiteSpace(text, ref index, ref lineNumber);
            return;
        }
        if (char.IsWhiteSpace(text[index]))
        {
            index++;
            SkipWhiteSpace(text, ref index, ref lineNumber);
            return;
        }
    }

    private static IToken? GetSingleLineCommentToken(string text, ref int index, ref int lineNumber)
    {
        int startIndex = index;
        int i;
        if (text.Length - startIndex < 2)
            return null;
        if (text[startIndex] != '/')
            return null;
        if (text[startIndex + 1] != '/')
            return null;
        i = startIndex + 2;
        while (i < text.Length && text[i] != '\n')
            i++;
        index = i;
        return new SingleLineCommentToken(text[startIndex..index], lineNumber);
    }

    private static IToken? GetMultiLineCommentToken(string text, ref int index, ref int lineNumber)
    {
        int startIndex = index;
        int i;
        int originalLineNumber = lineNumber;
        int lineNumberTemp = lineNumber;
        if (text.Length - startIndex < 2)
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
        index = i;
        lineNumber = lineNumberTemp;
        if (!closed)
            return new InvalidToken(text[startIndex..], originalLineNumber,
                new($"Multi-line comment starting at line {originalLineNumber} is never closed"));
        return new MultiLineCommentToken(text[startIndex..index], originalLineNumber);
    }

    private static InvalidToken GetInvalidToken(string text, ref int index, ref int lineNumber)
    {
        int startIndex = index;
        SkipToWhiteSpace(text, ref index);
        string tokenText = text[startIndex..index];
        return new InvalidToken(tokenText, lineNumber,
            new($"Invalid token \"{tokenText}\""));
    }

    private static void SkipToWhiteSpace(string text, ref int index)
    {
        if (index >= text.Length)
            return;
        if (!char.IsWhiteSpace(text[index]))
        {
            index++;
            SkipToWhiteSpace(text, ref index);
        }
    }
}