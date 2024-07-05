using System.Collections.Generic;
using System.Linq;

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
                GetLiteralToken(text, ref index, ref lineNumber) ??
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

    private static IToken? GetLiteralToken(string text, ref int index, ref int lineNumber)
    {
        return
            GetBooleanLiteralToken(text, ref index, ref lineNumber) ??
            GetIntegerLiteralToken(text, ref index, ref lineNumber) ??
            GetDoubleFloatLiteralToken(text, ref index, ref lineNumber) ??
            GetCharacterLiteralToken(text, ref index, ref lineNumber);
    }

    private static IToken? GetBooleanLiteralToken(string text, ref int index, ref int lineNumber)
    {
        const int TrueLength = 4;
        const int FalseLength = 5;
        if (text.Length - index < TrueLength)
            return null;
        if (text[index..(index + TrueLength)] == "true")
        {
            index += TrueLength;
            return new BooleanLiteralToken("true", true, lineNumber);
        }
        if (text.Length - index < FalseLength)
            return null;
        if (text[index..(index + FalseLength)] == "false")
        {
            index += FalseLength;
            return new BooleanLiteralToken("false", false, lineNumber);
        }
        return null;
    }

    private static IToken? GetIntegerLiteralToken(string text, ref int index, ref int lineNumber)
    {
        int i = index;
        while (i < text.Length)
        {
            if (text[i] == '.')
                return null;
            if (!char.IsDigit(text[i]))
                break;
            i++;
        }
        string tokenText = text[index..i];
        bool success = int.TryParse(tokenText, out int result);
        if (!success)
            return null;
        index = i;
        return new IntegerLiteralToken(tokenText, result, lineNumber);
    }

    private static IToken? GetDoubleFloatLiteralToken(string text, ref int index, ref int lineNumber)
    {
        int i = index;
        while (i < text.Length)
        {
            if (!char.IsDigit(text[i]) && text[i] != '.')
                break;
            i++;
        }
        string tokenText = text[index..i];
        bool success = double.TryParse(tokenText, out double result);
        if (!success)
            return null;
        index = i;
        return new DoubleFloatLiteralToken(tokenText, result, lineNumber);
    }

    private static IToken? GetCharacterLiteralToken(string text, ref int index, ref int lineNumber)
    {
        if (text.Length - index < 3)
            return null;
        if (text[index] != '\'')
            return null;
        int closingQuoteIndex =
            (text.Length - index >= 4 && text[index + 3] == '\'') ? index + 3 :
            text[index + 2] == '\'' ? index + 2 :
            -1;
        if (closingQuoteIndex == -1)
            return new InvalidToken(text[index..(index + 3)], lineNumber,
                new($"Quote not closed or too many characters in quote at line {lineNumber}"));
        if (closingQuoteIndex == index + 2)
        {
            char value = text[index + 1];
            if ((new char[] { '\'', '\\', '\n', '\r', '\t', '\v' }).Contains(value))
                return new InvalidToken(text[index..(index + 3)], lineNumber,
                    new($"Cannot have single character {value} in char quote (line {lineNumber})"));
            string tokenText = text[index..(index + 3)];
            index += 3;
            return new CharacterLiteralToken(tokenText, value, lineNumber);
        }
        if (text[index + 1] != '\\')
            return new InvalidToken(text[index..(index + 4)], lineNumber,
                new($"First character in quote must be \\ (line {lineNumber})"));
        if (!CharUtils.BasicEscapeCharacters.ContainsKey(text[(index + 1)..(index + 3)]))
        {
            char value = text[index + 2];
            if ((new char[] { '\n', '\r', '\t', '\v' }).Contains(value))
                return new InvalidToken(text[index..(index + 4)], lineNumber,
                    new($"Cannot have character {value} after \\ in quote (line {lineNumber})"));
            string tokenText = text[index..(index + 4)];
            index += 4;
            return new CharacterLiteralToken(tokenText, value, lineNumber);
        }
        char value_ = CharUtils.BasicEscapeCharacters[text[(index + 1)..(index + 3)]];
        string tokenText_ = text[index..(index + 4)];
        index += 4;
        return new CharacterLiteralToken(tokenText_, value_, lineNumber);
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