using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace CREInterpreter.Tokens;

public static class TokenSeparator
{
    public static IEnumerable<IToken> GetTokens(ReadOnlyMemory<char> text)
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
                GetSymbolToken(text, ref index, ref lineNumber) ??
                GetKeywordToken(text, ref index, ref lineNumber) ??
                GetTypeNameToken(text, ref index, ref lineNumber) ??
                GetVariableNameToken(text, ref index, ref lineNumber) ??
                GetInvalidToken(text, ref index, ref lineNumber);
        }
    }

    private static void SkipWhiteSpace(ReadOnlyMemory<char> text, ref int index, ref int lineNumber)
    {
        ReadOnlySpan<char> textSpan = text.Span;
        if (index >= text.Length)
            return;
        if (textSpan[index] == '\n')
        {
            index++;
            lineNumber++;
            SkipWhiteSpace(text, ref index, ref lineNumber);
            return;
        }
        if (char.IsWhiteSpace(textSpan[index]))
        {
            index++;
            SkipWhiteSpace(text, ref index, ref lineNumber);
            return;
        }
    }

    private static IToken? GetSingleLineCommentToken(ReadOnlyMemory<char> chunkText, ref int index, ref int lineNumber)
    {
        int startIndex = index;
        ReadOnlySpan<char> textSpan = chunkText.Span[index..];
        if (textSpan is not ['/', '/', ..])
            return null;
        int newlineOffset = textSpan.IndexOf('\n');
        if (newlineOffset == -1)
        {
            index = chunkText.Length;
            return new SingleLineCommentToken(chunkText[startIndex..], lineNumber, startIndex);
        }
        index += newlineOffset;
        return new SingleLineCommentToken(chunkText[startIndex..index], lineNumber, startIndex);
    }

    private static IToken? GetMultiLineCommentToken(ReadOnlyMemory<char> chunkText, ref int index, ref int lineNumber)
    {
        int startIndex = index;
        int startLineNumber = lineNumber;
        ReadOnlySpan<char> textSpan = chunkText.Span[index..];
        if (textSpan is not ['/', '*', ..])
            return null;
        for (int i = 2; i < textSpan.Length; i++)
        {
            if (textSpan[i] == '\n')
            {
                lineNumber++;
                continue;
            }
            bool isClosingSlash =
                i >= 3 &&
                textSpan[i] == '/' &&
                textSpan[i - 1] == '*';
            if (isClosingSlash)
            {
                int offset = i + 1;
                index += offset;
                return new MultiLineCommentToken(chunkText[startIndex..index], startLineNumber, startIndex);
            }
        }
        index = chunkText.Length;
        return new InvalidToken(chunkText[startIndex..], startLineNumber, startIndex, new("Multi-line comment never closed"));
    }

    private static IToken? GetLiteralToken(ReadOnlyMemory<char> chunkText, ref int index, ref int lineNumber)
    {
        return
            GetBooleanLiteralToken(chunkText, ref index, ref lineNumber) ??
            GetIntegerLiteralToken(chunkText, ref index, ref lineNumber) ??
            GetDoubleFloatLiteralToken(chunkText, ref index, ref lineNumber) ??
            GetCharacterLiteralToken(chunkText, ref index, ref lineNumber) ??
            GetStringLiteralToken(chunkText, ref index, ref lineNumber);
    }

    private static IToken? GetBooleanLiteralToken(ReadOnlyMemory<char> chunkText, ref int index, ref int lineNumber)
    {
        int startIndex = index;
        ReadOnlySpan<char> textSpan = chunkText.Span[index..];
        if (textSpan is ['t', 'r', 'u', 'e', ..])
        {
            index += 4;
            return new BooleanLiteralToken(chunkText[startIndex..index], true, lineNumber, startIndex);
        }
        if (textSpan is ['f', 'a', 'l', 's', 'e', ..])
        {
            index += 5;
            return new BooleanLiteralToken(chunkText[startIndex..index], false, lineNumber, startIndex);
        }
        return null;
    }

    private static IToken? GetIntegerLiteralToken(ReadOnlyMemory<char> chunkText, ref int index, ref int lineNumber)
    {
        int startIndex = index;
        ReadOnlySpan<char> textSpan = chunkText.Span[index..];
        int i = 0;
        while (i < textSpan.Length)
        {
            if (textSpan[i] == '.')
                return null;
            if (!char.IsDigit(textSpan[i]))
                break;
            i++;
        }
        ReadOnlySpan<char> reducedTextSpan = textSpan[..i];
        bool success = int.TryParse(reducedTextSpan, out int result);
        if (!success)
            return null;
        index += i;
        return new IntegerLiteralToken(chunkText[startIndex..index], result, lineNumber, startIndex);
    }

    private static IToken? GetDoubleFloatLiteralToken(ReadOnlyMemory<char> chunkText, ref int index, ref int lineNumber)
    {
        int startIndex = index;
        ReadOnlySpan<char> textSpan = chunkText.Span[index..];
        int i = 0;
        while (i < textSpan.Length)
        {
            if (!char.IsDigit(textSpan[i]) && textSpan[i] != '.')
                break;
            i++;
        }
        ReadOnlySpan<char> reducedTextSpan = textSpan[..i];
        bool success = double.TryParse(reducedTextSpan, out double result);
        if (!success)
            return null;
        index += i;
        return new DoubleFloatLiteralToken(chunkText[startIndex..index], result, lineNumber, startIndex);
    }

    private static IToken? GetCharacterLiteralToken(ReadOnlyMemory<char> chunkText, ref int index, ref int lineNumber)
    {
        int startIndex = index;
        ReadOnlySpan<char> textSpan = chunkText.Span[index..];
        return textSpan switch
        {
            ['\'', '\'', ..] => GetBadToken(2, "Single quotes cannot be empty", ref index, ref lineNumber),
            ['\'', '\0' or '\a' or '\b' or '\f' or '\n' or '\r' or '\t' or '\v', ..] =>
                GetBadToken(1, "Invalid character (such as newline) after opening single quote", ref index, ref lineNumber),
            ['\'', '\\', '\0' or '\a' or '\b' or '\f' or '\n' or '\r' or '\t' or '\v', ..] =>
                GetBadToken(2, "Invalid character (such as newline) after opening single quote", ref index, ref lineNumber),
            ['\'', not '\\', not '\'', ..] => GetBadToken(2, "Single quote not closed or quote contains too many characters", ref index, ref lineNumber),
            ['\'', '\\', _, not '\'', ..] => GetBadToken(3, "Single quote not closed or quote contains too many characters", ref index, ref lineNumber),
            ['\'', not ('\\' or '\''), '\'', ..] => GetToken(3, textSpan[1], ref index, ref lineNumber),
            ['\'', '\\', '\'', '\'', ..] => GetToken(4, '\'', ref index, ref lineNumber),
            ['\'', '\\', '"', '\'', ..] => GetToken(4, '"', ref index, ref lineNumber),
            ['\'', '\\', '\\', '\'', ..] => GetToken(4, '\\', ref index, ref lineNumber),
            ['\'', '\\', '0', '\'', ..] => GetToken(4, '\0', ref index, ref lineNumber),
            ['\'', '\\', 'a', '\'', ..] => GetToken(4, '\a', ref index, ref lineNumber),
            ['\'', '\\', 'b', '\'', ..] => GetToken(4, '\b', ref index, ref lineNumber),
            ['\'', '\\', 'f', '\'', ..] => GetToken(4, '\f', ref index, ref lineNumber),
            ['\'', '\\', 'n', '\'', ..] => GetToken(4, '\n', ref index, ref lineNumber),
            ['\'', '\\', 'r', '\'', ..] => GetToken(4, '\r', ref index, ref lineNumber),
            ['\'', '\\', 't', '\'', ..] => GetToken(4, '\t', ref index, ref lineNumber),
            ['\'', '\\', 'v', '\'', ..] => GetToken(4, '\v', ref index, ref lineNumber),
            ['\'', '\\', char badEscape, '\'', ..] => GetBadToken(4, $"Unrecognised escape sequence: \\{badEscape}", ref index, ref lineNumber),
            _ => null
        };

        CharacterLiteralToken GetToken(int length, char value, ref int index, ref int lineNumber) =>
            new(chunkText[startIndex..(index += length)], value, lineNumber, startIndex);

        InvalidToken GetBadToken(int length, string message, ref int index, ref int lineNumber) =>
            new(chunkText[startIndex..(index += length)], lineNumber, startIndex, new(message));
    }

    private static IToken? GetStringLiteralToken(ReadOnlyMemory<char> chunkText, ref int index, ref int lineNumber)
    {
        int startIndex = index;
        ReadOnlySpan<char> textSpan = chunkText.Span[index..];
        if (textSpan.Length < 2)
            return null;
        if (textSpan[0] != '"')
            return null;
        int i = 1;
        for (; i < textSpan.Length; i++)
        {
            if (textSpan[i] == '"')
                break;
            bool badChar = textSpan[i] switch
            {
                '\0' or '\a' or '\b' or '\f' or '\n' or '\r' or '\t' or '\v' => true,
                _ => false
            };
            if (badChar)
                return new InvalidToken(chunkText[startIndex..(index += i)], lineNumber, startIndex,
                    new("String literal contains invalid character (such as a newline)"));
        }
        if (i == textSpan.Length)
        {
            index = i;
            return new InvalidToken(chunkText[startIndex..], lineNumber, startIndex, new("Quote never closed"));
        }
        index = i + 1;
        return new StringLiteralToken(chunkText[startIndex..index], chunkText[(startIndex + 1)..(index - 1)], lineNumber, startIndex);
    }

    private static IToken? GetSymbolToken(ReadOnlyMemory<char> chunkText, ref int index, ref int lineNumber)
    {
        int startIndex = index;
        ReadOnlySpan<char> textSpan = chunkText.Span[index..];
        string? symbol = null;
        foreach (string _symbol in SymbolMappings.Keys)
            if (textSpan.StartsWith(_symbol))
            {
                symbol = _symbol;
                break;
            }
        if (symbol == null)
            return null;
        Func<ReadOnlyMemory<char>, int, int, IToken> tokenCreator = SymbolMappings[symbol];
        index += symbol.Length;
        return tokenCreator(chunkText[startIndex..index], lineNumber, startIndex);
    }

    private class StringSizeComp : IComparer<string>
    {
        // sort strings by size, from biggest to smallest
        public int Compare(string? x, string? y) => (x, y) switch
        {
            (null, null) => 0,
            (null, not null) => -1,
            (not null, null) => 1,
            (not null, not null) =>
                y.Length != x.Length ?
                y.Length - x.Length :
                x.CompareTo(y)
        };
    }

    // when checking if a given symbol matches one of the keys, we check the bigger symbols (i.e. with bigger string length) first
    // this is so that when a symbol is a substring of another symbol, the bigger symbol is able to be picked
    // for example the symbol "<" is contained in the symbol "<=", so just by looking at the first character of "<="
    //      one may think that the whole symbol is "<"
    // therefore we check if the symbol is "<=" before checking if it is "<" to obtain the correct symbol
    // the symbols below are already ordered in this way, however the StringSizeComp adds another runtime sort to ensure this
    private static ImmutableSortedDictionary<string, Func<ReadOnlyMemory<char>, int, int, IToken>> SymbolMappings { get; } =
        ImmutableSortedDictionary.CreateRange(new StringSizeComp(),
            new Dictionary<string, Func<ReadOnlyMemory<char>, int, int, IToken>>()
            {
                ["<="] =    (s, l, i) => new LessThanOrEqualToSymbolToken(s, l, i),
                [">="] =    (s, l, i) => new GreaterThanOrEqualToSymbolToken(s, l, i),
                ["&&"] =    (s, l, i) => new ConditionalAndSymbolToken(s, l, i),
                ["||"] =    (s, l, i) => new ConditionalOrSymbolToken(s, l, i),
                ["=="] =    (s, l, i) => new EqualsEqualsSymbolToken(s, l, i),
                ["!="] =    (s, l, i) => new NotEqualsSymbolToken(s, l, i),
                ["="] =     (s, l, i) => new EqualsSymbolToken(s, l, i),
                ["{"] =     (s, l, i) => new OpenCurlyBraceSymbolToken(s, l, i),
                ["}"] =     (s, l, i) => new CloseCurlyBraceSymbolToken(s, l, i),
                [","] =     (s, l, i) => new CommaSymbolToken(s, l, i),
                ["."] =     (s, l, i) => new DotSymbolToken(s, l, i),
                ["+"] =     (s, l, i) => new PlusSymbolToken(s, l, i),
                ["-"] =     (s, l, i) => new MinusSymbolToken(s, l, i),
                ["*"] =     (s, l, i) => new MultiplySymbolToken(s, l, i),
                ["/"] =     (s, l, i) => new DivideSymbolToken(s, l, i),
                ["<"] =     (s, l, i) => new LessThanSymbolToken(s, l, i),
                [">"] =     (s, l, i) => new GreaterThanSymbolToken(s, l, i),
                ["("] =     (s, l, i) => new OpenBracketSymbolToken(s, l, i),
                [")"] =     (s, l, i) => new CloseBracketSymbolToken(s, l, i),
                ["!"] =     (s, l, i) => new NotSymbolToken(s, l, i),
                ["&"] =     (s, l, i) => new AndSymbolToken(s, l, i),
                ["|"] =     (s, l, i) => new OrSymbolToken(s, l, i),
                ["^"] =     (s, l, i) => new XorSymbolToken(s, l, i),
                ["%"] =     (s, l, i) => new RemainderSymbolToken(s, l, i),
                [";"] =     (s, l, i) => new SemicolonSymbolToken(s, l, i),
                ["["] =     (s, l, i) => new OpenSquareBraceSymbolToken(s, l, i),
                ["]"] =     (s, l, i) => new CloseSquareBraceSymbolToken(s, l, i)
            });

    private static IToken? GetKeywordToken(ReadOnlyMemory<char> chunkText, ref int index, ref int lineNumber)
    {
        int startIndex = index;
        ReadOnlySpan<char> textSpan = chunkText.Span[index..];
        string? keyword = null;
        foreach (string _keyword in KeywordMappings.Keys)
            if (textSpan.StartsWith(_keyword))
            {
                keyword = _keyword;
                break;
            }
        if (keyword == null)
            return null;
        if (textSpan.Length == keyword.Length)
            return ReturnKeywordToken(ref index, ref lineNumber);
        char nextChar = textSpan[keyword.Length];
        if (char.IsLetterOrDigit(nextChar) || nextChar == '_')
            return null;
        return ReturnKeywordToken(ref index, ref lineNumber);

        IToken ReturnKeywordToken(ref int index, ref int lineNumber) =>
            KeywordMappings[keyword](chunkText[index..(index += keyword.Length)], lineNumber, startIndex);
    }

    private static ImmutableSortedDictionary<string, Func<ReadOnlyMemory<char>, int, int, IToken>> KeywordMappings { get; } =
        ImmutableSortedDictionary.CreateRange(new StringSizeComp(),
            new Dictionary<string, Func<ReadOnlyMemory<char>, int, int, IToken>>()
            {
                ["new"] =       (s, l, i) => new NewKeywordToken(s, l, i),
                ["null"] =      (s, l, i) => new NullKeywordToken(s, l, i),
                ["if"] =        (s, l, i) => new IfKeywordToken(s, l, i),
                ["else"] =      (s, l, i) => new ElseKeywordToken(s, l, i),
                ["while"] =     (s, l, i) => new WhileKeywordToken(s, l, i),
                ["for"] =       (s, l, i) => new ForKeywordToken(s, l, i),
                ["break"] =     (s, l, i) => new BreakKeywordToken(s, l, i),
                ["continue"] =  (s, l, i) => new ContinueKeywordToken(s, l, i),
                ["Length"] =    (s, l, i) => new LengthKeywordToken(s, l, i)
            });

    private static IToken? GetTypeNameToken(ReadOnlyMemory<char> text, ref int index, ref int lineNumber)
    {
        int startIndex = index;
        int i = index;
        ReadOnlySpan<char> textSpan = text.Span;
        VarType? varType = VarType.VarTypes
            .Where(varType => !varType.IsArray)
            .SingleOrDefault(varType =>
                i + varType.Name.Length <= text.Length &&
                text[i..(i + varType.Name.Length)].ToString() == varType.Name);
        if (varType == null)
            return null;
        if (i + varType.Name.Length < text.Length)
        {
            char nextChar = textSpan[i + varType.Name.Length];
            if (char.IsLetterOrDigit(nextChar) || nextChar == '_')
                return null;
        }
        index += varType.Name.Length;
        return new TypeNameToken(text[startIndex..index], varType, lineNumber, startIndex);
    }

    private static IToken? GetVariableNameToken(ReadOnlyMemory<char> text, ref int index, ref int lineNumber)
    {
        int startIndex = index;
        ReadOnlySpan<char> textSpan = text.Span;
        if (!char.IsLetter(textSpan[index]) && textSpan[index] != '_')
            return null;
        int i = index;
        while (i < text.Length && (char.IsLetterOrDigit(textSpan[i]) || textSpan[i] == '_'))
            i++;
        ReadOnlyMemory<char> tokenText = text[index..i];
        if (KeywordMappings.ContainsKey(tokenText.ToString()))
            return null;
        if (VarType.GetVarType(tokenText) != null)
            return null;
        index = i;
        return new VariableNameToken(tokenText, lineNumber, startIndex);
    }

    private static InvalidToken GetInvalidToken(ReadOnlyMemory<char> text, ref int index, ref int lineNumber)
    {
        int startIndex = index;
        SkipToWhiteSpace(text, ref index);
        ReadOnlyMemory<char> tokenText = text[startIndex..index];
        return new InvalidToken(tokenText, lineNumber, startIndex,
            new($"Invalid token \"{tokenText}\""));
    }

    private static void SkipToWhiteSpace(ReadOnlyMemory<char> text, ref int index)
    {
        if (index >= text.Length)
            return;
        ReadOnlySpan<char> textSpan = text.Span;
        if (!char.IsWhiteSpace(textSpan[index]))
        {
            index++;
            SkipToWhiteSpace(text, ref index);
        }
    }
}