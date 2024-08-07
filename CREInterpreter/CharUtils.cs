﻿using System.Collections.Generic;

namespace CREInterpreter;

public static class CharUtils
{
    public static Dictionary<string, char> BasicEscapeCharacters { get; } = new()
    {
        [@"\'"] = '\'',
        ["\\\""] = '"',
        [@"\\"] = '\\',
        [@"\0"] = '\0',
        [@"\a"] = '\a',
        [@"\b"] = '\b',
        [@"\f"] = '\f',
        [@"\n"] = '\n',
        [@"\r"] = '\r',
        [@"\t"] = '\t',
        [@"\v"] = '\v'
    };
}