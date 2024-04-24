using static CRECSharpInterpreter.VarType;
using System;

namespace CRECSharpInterpreter.Tests
{
    public class BadTest : ITest
    {
        public BadTest(string pathNoExt, Error error)
        {
            PathNoExt = pathNoExt;
            Error = error;
        }

        public string PathNoExt { get; init; }

        public Variable[][] Stack => Array.Empty<Variable[]>();

        public Variable?[] Heap => Array.Empty<Variable?>();

        public Error Error { get; init; }
    }
}
