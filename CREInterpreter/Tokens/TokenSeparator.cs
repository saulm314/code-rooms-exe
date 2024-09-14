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

    private static IToken? GetSingleLineCommentToken(ReadOnlyMemory<char> text, ref int index, ref int lineNumber)
    {
        int startIndex = index;
        int i;
        ReadOnlySpan<char> textSpan = text.Span;
        if (text.Length - startIndex < 2)
            return null;
        if (textSpan[startIndex] != '/')
            return null;
        if (textSpan[startIndex + 1] != '/')
            return null;
        i = startIndex + 2;
        while (i < text.Length && textSpan[i] != '\n')
            i++;
        index = i;
        return new SingleLineCommentToken(text[startIndex..index], lineNumber, startIndex);
    }

    private static IToken? GetMultiLineCommentToken(ReadOnlyMemory<char> text, ref int index, ref int lineNumber)
    {
        int startIndex = index;
        int i;
        int originalLineNumber = lineNumber;
        int lineNumberTemp = lineNumber;
        ReadOnlySpan<char> textSpan = text.Span;
        if (text.Length - startIndex < 2)
            return null;
        if (textSpan[startIndex] != '/')
            return null;
        if (textSpan[startIndex + 1] != '*')
            return null;
        i = startIndex + 2;
        bool previousIsAsterisk = false;
        bool closed = false;
        while (i < text.Length)
        {
            if (textSpan[i] == '*')
            {
                previousIsAsterisk = true;
                i++;
                continue;
            }
            if (textSpan[i] == '/' && previousIsAsterisk)
            {
                i++;
                closed = true;
                break;
            }
            if (textSpan[i] == '\n')
                lineNumberTemp++;
            previousIsAsterisk = false;
            i++;
        }
        index = i;
        lineNumber = lineNumberTemp;
        if (!closed)
            return new InvalidToken(text[startIndex..], originalLineNumber, startIndex,
                new($"Multi-line comment starting at line {originalLineNumber} is never closed"));
        return new MultiLineCommentToken(text[startIndex..index], originalLineNumber, startIndex);
    }

    private static IToken? GetLiteralToken(ReadOnlyMemory<char> text, ref int index, ref int lineNumber)
    {
        return
            GetBooleanLiteralToken(text, ref index, ref lineNumber) ??
            GetIntegerLiteralToken(text, ref index, ref lineNumber) ??
            GetDoubleFloatLiteralToken(text, ref index, ref lineNumber) ??
            GetCharacterLiteralToken(text, ref index, ref lineNumber) ??
            GetStringLiteralToken(text, ref index, ref lineNumber);
    }

    private static IToken? GetBooleanLiteralToken(ReadOnlyMemory<char> text, ref int index, ref int lineNumber)
    {
        int startIndex = index;
        ReadOnlySpan<char> textSpan = text.Span;
        const int TrueLength = 4;
        const int FalseLength = 5;
        if (text.Length - index < TrueLength)
            return null;
        if (textSpan[index..(index + TrueLength)].Equals("true".AsSpan(), default))
        {
            index += TrueLength;
            return new BooleanLiteralToken(text[startIndex..index], true, lineNumber, startIndex);
        }
        if (text.Length - index < FalseLength)
            return null;
        if (textSpan[index..(index + FalseLength)].Equals("false".AsSpan(), default))
        {
            index += FalseLength;
            return new BooleanLiteralToken(text[startIndex..index], false, lineNumber, startIndex);
        }
        return null;
    }

    private static IToken? GetIntegerLiteralToken(ReadOnlyMemory<char> text, ref int index, ref int lineNumber)
    {
        int startIndex = index;
        int i = index;
        ReadOnlySpan<char> textSpan = text.Span;
        while (i < text.Length)
        {
            if (textSpan[i] == '.')
                return null;
            if (!char.IsDigit(textSpan[i]))
                break;
            i++;
        }
        ReadOnlyMemory<char> tokenText = text[index..i];
        ReadOnlySpan<char> tokenTextSpan = tokenText.Span;
        bool success = int.TryParse(tokenTextSpan, out int result);
        if (!success)
            return null;
        index = i;
        return new IntegerLiteralToken(tokenText, result, lineNumber, startIndex);
    }

    private static IToken? GetDoubleFloatLiteralToken(ReadOnlyMemory<char> text, ref int index, ref int lineNumber)
    {
        int startIndex = index;
        int i = index;
        ReadOnlySpan<char> textSpan = text.Span;
        while (i < text.Length)
        {
            if (!char.IsDigit(textSpan[i]) && textSpan[i] != '.')
                break;
            i++;
        }
        ReadOnlyMemory<char> tokenText = text[index..i];
        ReadOnlySpan<char> tokenTextSpan = tokenText.Span;
        bool success = double.TryParse(tokenTextSpan, out double result);
        if (!success)
            return null;
        index = i;
        return new DoubleFloatLiteralToken(tokenText, result, lineNumber, startIndex);
    }

    // this code is horrible
    private static IToken? GetCharacterLiteralToken(ReadOnlyMemory<char> text, ref int index, ref int lineNumber)
    {
        int startIndex = index;
        ReadOnlySpan<char> textSpan = text.Span;
        if (text.Length - index < 3)
            return null;
        if (textSpan[index] != '\'')
            return null;
        int closingQuoteIndex =
            (text.Length - index >= 4 && textSpan[index + 3] == '\'') ? index + 3 :
            textSpan[index + 2] == '\'' ? index + 2 :
            -1;
        if (closingQuoteIndex == -1)
            return new InvalidToken(text[index..(index += 3)], lineNumber, startIndex,
                new($"Quote not closed or too many characters in quote at line {lineNumber}"));
        if (closingQuoteIndex == index + 2)
        {
            char value = textSpan[index + 1];
            if ((new char[] { '\'', '\\', '\n', '\r', '\t', '\v' }).Contains(value))
                return new InvalidToken(text[index..(index += 3)], lineNumber, startIndex,
                    new($"Cannot have single character {value} in char quote (line {lineNumber})"));
            ReadOnlyMemory<char> tokenText = text[index..(index + 3)];
            index += 3;
            return new CharacterLiteralToken(tokenText, value, lineNumber, startIndex);
        }
        if (textSpan[index + 1] != '\\')
            return new InvalidToken(text[index..(index += 4)], lineNumber, startIndex,
                new($"First character in quote must be \\ (line {lineNumber})"));
        string basicEscapeCharacter = text[(index + 1)..(index + 3)].ToString();
        if (!CharUtils.BasicEscapeCharacters.ContainsKey(basicEscapeCharacter))
        {
            char value = textSpan[index + 2];
            if ((new char[] { '\n', '\r', '\t', '\v' }).Contains(value))
                return new InvalidToken(text[index..(index += 4)], lineNumber, startIndex,
                    new($"Cannot have character {value} after \\ in quote (line {lineNumber})"));
            ReadOnlyMemory<char> tokenText = text[index..(index + 4)];
            index += 4;
            return new CharacterLiteralToken(tokenText, value, lineNumber, startIndex);
        }
        char value_ = CharUtils.BasicEscapeCharacters[basicEscapeCharacter];
        ReadOnlyMemory<char> tokenText_ = text[index..(index + 4)];
        index += 4;
        return new CharacterLiteralToken(tokenText_, value_, lineNumber, startIndex);
    }

    private static IToken? GetStringLiteralToken(ReadOnlyMemory<char> text, ref int index, ref int lineNumber)
    {
        int startIndex = index;
        int i;
        ReadOnlySpan<char> textSpan = text.Span;
        if (text.Length - startIndex < 2)
            return null;
        if (textSpan[startIndex] != '"')
            return null;
        i = startIndex + 1;
        while (i < text.Length && textSpan[i] != '\n' && textSpan[i] != '"')
            i++;
        index = i;
        if (i == text.Length)
            return new InvalidToken(text[startIndex..index], lineNumber, startIndex,
                new InterpreterException($"Quote at line {lineNumber} never closed"));
        if (textSpan[i] == '\n')
            return new InvalidToken(text[startIndex..index], lineNumber, startIndex,
                new InterpreterException($"Cannot have a newline mid-quote (line {lineNumber})"));
        index++;
        return new StringLiteralToken(text[startIndex..index], text[(startIndex + 1)..(index - 1)], lineNumber, startIndex);
    }

    private static IToken? GetSymbolToken(ReadOnlyMemory<char> text, ref int index, ref int lineNumber)
    {
        int startIndex = index;
        int i = index;
        string[] possibleSymbols = SymbolMappings.Keys.ToArray();
        Array.Sort(possibleSymbols, new StringSizeComp());
        string? symbol = Array.Find(possibleSymbols, _symbol =>
            i + _symbol.Length <= text.Length &&
            text[i..(i + _symbol.Length)].ToString() == _symbol);
        if (symbol == null)
            return null;
        Func<ReadOnlyMemory<char>, int, int, IToken> tokenCreator = SymbolMappings[symbol];
        index += symbol.Length;
        return tokenCreator(text[startIndex..index], lineNumber, startIndex);
    }

    private class StringSizeComp : IComparer<string>
    {
        // sort strings by size, from biggest to smallest
        public int Compare(string? x, string? y) => (x, y) switch
        {
            (null, null) => 0,
            (null, not null) => -1,
            (not null, null) => 1,
            (not null, not null) => y.Length - x.Length
        };
    }

    // when checking if a given symbol matches one of the keys, we check the bigger symbols (i.e. with bigger string length) first
    // this is so that when a symbol is a substring of another symbol, the bigger symbol is able to be picked
    // for example the symbol "<" is contained in the symbol "<=", so just by looking at the first character of "<="
    //      one may think that the whole symbol is "<"
    // therefore we check if the symbol is "<=" before checking if it is "<" to obtain the correct symbol
    // the symbols below are already ordered in this way, however the StringSizeComp adds another runtime sort to ensure this
    private static ImmutableDictionary<string, Func<ReadOnlyMemory<char>, int, int, IToken>> SymbolMappings { get; } =
        ImmutableDictionary<string, Func<ReadOnlyMemory<char>, int, int, IToken>>.Empty.AddRange(
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

    private static IToken? GetKeywordToken(ReadOnlyMemory<char> text, ref int index, ref int lineNumber)
    {
        int startIndex = index;
        int i = index;
        ReadOnlySpan<char> textSpan = text.Span;
        string[] possibleKeywords = KeywordMappings.Keys.ToArray();
        string? keyword = Array.Find(possibleKeywords, _keyword =>
            i + _keyword.Length <= text.Length &&
            text[i..(i + _keyword.Length)].ToString() == _keyword);
        if (keyword == null)
            return null;
        if (i + keyword.Length < text.Length)
        {
            char nextChar = textSpan[i + keyword.Length];
            if (char.IsLetterOrDigit(nextChar) || nextChar == '_')
                return null;
        }
        Func<ReadOnlyMemory<char>, int, int, IToken> tokenCreator = KeywordMappings[keyword];
        index += keyword.Length;
        return tokenCreator(text[startIndex..index], lineNumber, startIndex);
    }

    private static ImmutableDictionary<string, Func<ReadOnlyMemory<char>, int, int, IToken>> KeywordMappings { get; } =
        ImmutableDictionary<string, Func<ReadOnlyMemory<char>, int, int, IToken>>.Empty.AddRange(
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