﻿using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests
{
    public class WriteVar : ITest
    {
        public string Path => @"Variables\WriteVar";

        public Variable[][] Stack =>
            new[]
            {
                new Variable[]
                {
                    new(@int,   "myInt",     5,     true),
                    new(@int,   "myInt2",    5,     true),
                    new(@int,   "myInt3",   -7,     true),
                    new(@int,   "myInt4",   -7,     true),
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
