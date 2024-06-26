﻿using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.Good
{
    public class BooleanConditionalOr : ITest
    {
        public BooleanConditionalOr(string pathNoExt)
        {
            PathNoExt = pathNoExt;
        }

        public string PathNoExt { get; init; }

        public Variable[][] Stack => 
            new[]
            {
                new Variable[]
                {
                    new(@bool, "myB", true, true),
                    new(@bool, "myB2", true, true),
                    new(@bool, "myB3", true, true),
                    new(@bool, "myB4", false, true),
                    new(@bool.Array, "bArr", null, true),
                    new(@bool, "myB5", true, true)
                }
            };

        public Variable?[] Heap =>
            new Variable?[]
            {
                new(null)
            };

        public Error Error { get; } = Error.None;
    }
}
