﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
                GetSymbolToken(text, ref index, ref lineNumber) ??
                GetKeywordToken(text, ref index, ref lineNumber) ??
                GetTypeNameToken(text, ref index, ref lineNumber) ??
                GetVariableNameToken(text, ref index, ref lineNumber) ??
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
        return new SingleLineCommentToken(text[startIndex..index], lineNumber, startIndex);
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
            return new InvalidToken(text[startIndex..], originalLineNumber, startIndex,
                new($"Multi-line comment starting at line {originalLineNumber} is never closed"));
        return new MultiLineCommentToken(text[startIndex..index], originalLineNumber, startIndex);
    }

    private static IToken? GetLiteralToken(string text, ref int index, ref int lineNumber)
    {
        return
            GetBooleanLiteralToken(text, ref index, ref lineNumber) ??
            GetIntegerLiteralToken(text, ref index, ref lineNumber) ??
            GetDoubleFloatLiteralToken(text, ref index, ref lineNumber) ??
            GetCharacterLiteralToken(text, ref index, ref lineNumber) ??
            GetStringLiteralToken(text, ref index, ref lineNumber);
    }

    private static IToken? GetBooleanLiteralToken(string text, ref int index, ref int lineNumber)
    {
        int startIndex = index;
        const int TrueLength = 4;
        const int FalseLength = 5;
        if (text.Length - index < TrueLength)
            return null;
        if (text[index..(index + TrueLength)] == "true")
        {
            index += TrueLength;
            return new BooleanLiteralToken("true", true, lineNumber, startIndex);
        }
        if (text.Length - index < FalseLength)
            return null;
        if (text[index..(index + FalseLength)] == "false")
        {
            index += FalseLength;
            return new BooleanLiteralToken("false", false, lineNumber, startIndex);
        }
        return null;
    }

    private static IToken? GetIntegerLiteralToken(string text, ref int index, ref int lineNumber)
    {
        int startIndex = index;
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
        return new IntegerLiteralToken(tokenText, result, lineNumber, startIndex);
    }

    private static IToken? GetDoubleFloatLiteralToken(string text, ref int index, ref int lineNumber)
    {
        int startIndex = index;
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
        return new DoubleFloatLiteralToken(tokenText, result, lineNumber, startIndex);
    }

    private static IToken? GetCharacterLiteralToken(string text, ref int index, ref int lineNumber)
    {
        int startIndex = index;
        if (text.Length - index < 3)
            return null;
        if (text[index] != '\'')
            return null;
        int closingQuoteIndex =
            (text.Length - index >= 4 && text[index + 3] == '\'') ? index + 3 :
            text[index + 2] == '\'' ? index + 2 :
            -1;
        if (closingQuoteIndex == -1)
            return new InvalidToken(text[index..(index += 3)], lineNumber, startIndex,
                new($"Quote not closed or too many characters in quote at line {lineNumber}"));
        if (closingQuoteIndex == index + 2)
        {
            char value = text[index + 1];
            if ((new char[] { '\'', '\\', '\n', '\r', '\t', '\v' }).Contains(value))
                return new InvalidToken(text[index..(index += 3)], lineNumber, startIndex,
                    new($"Cannot have single character {value} in char quote (line {lineNumber})"));
            string tokenText = text[index..(index + 3)];
            index += 3;
            return new CharacterLiteralToken(tokenText, value, lineNumber, startIndex);
        }
        if (text[index + 1] != '\\')
            return new InvalidToken(text[index..(index += 4)], lineNumber, startIndex,
                new($"First character in quote must be \\ (line {lineNumber})"));
        if (!CharUtils.BasicEscapeCharacters.ContainsKey(text[(index + 1)..(index + 3)]))
        {
            char value = text[index + 2];
            if ((new char[] { '\n', '\r', '\t', '\v' }).Contains(value))
                return new InvalidToken(text[index..(index += 4)], lineNumber, startIndex,
                    new($"Cannot have character {value} after \\ in quote (line {lineNumber})"));
            string tokenText = text[index..(index + 4)];
            index += 4;
            return new CharacterLiteralToken(tokenText, value, lineNumber, startIndex);
        }
        char value_ = CharUtils.BasicEscapeCharacters[text[(index + 1)..(index + 3)]];
        string tokenText_ = text[index..(index + 4)];
        index += 4;
        return new CharacterLiteralToken(tokenText_, value_, lineNumber, startIndex);
    }

    private static IToken? GetStringLiteralToken(string text, ref int index, ref int lineNumber)
    {
        int startIndex = index;
        int i;
        if (text.Length - startIndex < 2)
            return null;
        if (text[startIndex] != '"')
            return null;
        i = startIndex + 1;
        while (i < text.Length && text[i] != '\n' && text[i] != '"')
            i++;
        index = i;
        if (i == text.Length)
            return new InvalidToken(text[startIndex..index], lineNumber, startIndex,
                new InterpreterException($"Quote at line {lineNumber} never closed"));
        if (text[i] == '\n')
            return new InvalidToken(text[startIndex..index], lineNumber, startIndex,
                new InterpreterException($"Cannot have a newline mid-quote (line {lineNumber})"));
        index++;
        return new StringLiteralToken(text[startIndex..index], text[(startIndex + 1)..(index - 1)], lineNumber, startIndex);
    }

    private static IToken? GetSymbolToken(string text, ref int index, ref int lineNumber)
    {
        int startIndex = index;
        int i = index;
        string[] possibleSymbols = SymbolMappings.Keys.ToArray();
        Array.Sort(possibleSymbols, new StringSizeComp());
        string? symbol = Array.Find(possibleSymbols, _symbol =>
            i + _symbol.Length <= text.Length &&
            text[i..(i + _symbol.Length)] == _symbol);
        if (symbol == null)
            return null;
        Func<int, int, IToken> tokenCreator = SymbolMappings[symbol];
        index += symbol.Length;
        return tokenCreator(lineNumber, startIndex);
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
    private static ImmutableDictionary<string, Func<int, int, IToken>> SymbolMappings { get; } =
        ImmutableDictionary<string, Func<int, int, IToken>>.Empty.AddRange(
            new Dictionary<string, Func<int, int, IToken>>()
            {
                ["<="] =    (l, i) => new LessThanOrEqualToSymbolToken(l, i),
                [">="] =    (l, i) => new GreaterThanOrEqualToSymbolToken(l, i),
                ["&&"] =    (l, i) => new ConditionalAndSymbolToken(l, i),
                ["||"] =    (l, i) => new ConditionalOrSymbolToken(l, i),
                ["=="] =    (l, i) => new EqualsEqualsSymbolToken(l, i),
                ["!="] =    (l, i) => new NotEqualsSymbolToken(l, i),
                ["="] =     (l, i) => new EqualsSymbolToken(l, i),
                ["{"] =     (l, i) => new OpenCurlyBraceSymbolToken(l, i),
                ["}"] =     (l, i) => new CloseCurlyBraceSymbolToken(l, i),
                [","] =     (l, i) => new CommaSymbolToken(l, i),
                ["."] =     (l, i) => new DotSymbolToken(l, i),
                ["+"] =     (l, i) => new PlusSymbolToken(l, i),
                ["-"] =     (l, i) => new MinusSymbolToken(l, i),
                ["*"] =     (l, i) => new MultiplySymbolToken(l, i),
                ["/"] =     (l, i) => new DivideSymbolToken(l, i),
                ["<"] =     (l, i) => new LessThanSymbolToken(l, i),
                [">"] =     (l, i) => new GreaterThanSymbolToken(l, i),
                ["("] =     (l, i) => new OpenBracketSymbolToken(l, i),
                [")"] =     (l, i) => new CloseBracketSymbolToken(l, i),
                ["!"] =     (l, i) => new NotSymbolToken(l, i),
                ["&"] =     (l, i) => new AndSymbolToken(l, i),
                ["|"] =     (l, i) => new OrSymbolToken(l, i),
                ["^"] =     (l, i) => new XorSymbolToken(l, i),
                ["%"] =     (l, i) => new RemainderSymbolToken(l, i),
                [";"] =     (l, i) => new SemicolonSymbolToken(l, i),
                ["["] =     (l, i) => new OpenSquareBraceSymbolToken(l, i),
                ["]"] =     (l, i) => new CloseSquareBraceSymbolToken(l, i)
            });

    private static IToken? GetKeywordToken(string text, ref int index, ref int lineNumber)
    {
        int startIndex = index;
        int i = index;
        string[] possibleKeywords = KeywordMappings.Keys.ToArray();
        string? keyword = Array.Find(possibleKeywords, _keyword =>
            i + _keyword.Length <= text.Length &&
            text[i..(i + _keyword.Length)] == _keyword);
        if (keyword == null)
            return null;
        if (i + keyword.Length < text.Length)
        {
            char nextChar = text[i + keyword.Length];
            if (char.IsLetterOrDigit(nextChar) || nextChar == '_')
                return null;
        }
        Func<int, int, IToken> tokenCreator = KeywordMappings[keyword];
        index += keyword.Length;
        return tokenCreator(lineNumber, startIndex);
    }

    private static ImmutableDictionary<string, Func<int, int, IToken>> KeywordMappings { get; } =
        ImmutableDictionary<string, Func<int, int, IToken>>.Empty.AddRange(
            new Dictionary<string, Func<int, int, IToken>>()
            {
                ["new"] =       (l, i) => new NewKeywordToken(l, i),
                ["null"] =      (l, i) => new NullKeywordToken(l, i),
                ["if"] =        (l, i) => new IfKeywordToken(l, i),
                ["else"] =      (l, i) => new ElseKeywordToken(l, i),
                ["while"] =     (l, i) => new WhileKeywordToken(l, i),
                ["for"] =       (l, i) => new ForKeywordToken(l, i),
                ["break"] =     (l, i) => new BreakKeywordToken(l, i),
                ["continue"] =  (l, i) => new ContinueKeywordToken(l, i),
                ["Length"] =    (l, i) => new LengthKeywordToken(l, i)
            });

    private static IToken? GetTypeNameToken(string text, ref int index, ref int lineNumber)
    {
        int startIndex = index;
        int i = index;
        VarType? varType = VarType.VarTypes
            .Where(varType => !varType.IsArray)
            .SingleOrDefault(varType =>
                i + varType.Name.Length <= text.Length &&
                text[i..(i + varType.Name.Length)] == varType.Name);
        if (varType == null)
            return null;
        if (i + varType.Name.Length < text.Length)
        {
            char nextChar = text[i + varType.Name.Length];
            if (char.IsLetterOrDigit(nextChar) || nextChar == '_')
                return null;
        }
        index += varType.Name.Length;
        return new TypeNameToken(varType.Name, varType, lineNumber, startIndex);
    }

    private static IToken? GetVariableNameToken(string text, ref int index, ref int lineNumber)
    {
        int startIndex = index;
        if (!char.IsLetter(text[index]) && text[index] != '_')
            return null;
        int i = index;
        while (i < text.Length && (char.IsLetterOrDigit(text[i]) || text[i] == '_'))
            i++;
        string tokenText = text[index..i];
        if (KeywordMappings.ContainsKey(tokenText))
            return null;
        if (VarType.GetVarType(tokenText) != null)
            return null;
        index = i;
        return new VariableNameToken(tokenText, lineNumber, startIndex);
    }

    private static InvalidToken GetInvalidToken(string text, ref int index, ref int lineNumber)
    {
        int startIndex = index;
        SkipToWhiteSpace(text, ref index);
        string tokenText = text[startIndex..index];
        return new InvalidToken(tokenText, lineNumber, startIndex,
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