﻿using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.Types
{
    public class Char : ITest
    {
        public string Path => @"Types\Char";

        public Variable[][] Stack =>
            new[]
            {
                new Variable[]
                {
                    new(@char, "myChar", '\0',  false),
                    new(@char, "myChar2", '\0', true),
                    new(@char, "myChar3", '\'', true),
                    new(@char, "myChar4", '\"', true),
                    new(@char, "myChar5", '\\', true),
                    new(@char, "myChar6", '\a', true),
                    new(@char, "myChar7", '\b', true),
                    new(@char, "myChar8", '\f', true),
                    new(@char, "myChar9", '\n', true),
                    new(@char, "myChar10", '\r', true),
                    new(@char, "myChar11", '\t', true),
                    new(@char, "myChar12", '\v', true),
                    new(@char, "myChar13", 'a', true),
                    new(@char, "myChar14", '3', true),
                }
            };

        public Variable[] Heap =>
            new Variable[]
            {
                new(null)
            };

        public Error Error => Error.None;
    }
}
