using static CRECSharpInterpreter.VarType;
using System;

namespace CRECSharpInterpreter.Tests
{
    public class GoodTestNotFound : ITest
    {
        public GoodTestNotFound(string pathNoExt)
        {
            PathNoExt = pathNoExt;
        }

        public string PathNoExt { get; init; }

        public Variable[][] Stack => Array.Empty<Variable[]>();

        public Variable[] Heap => Array.Empty<Variable>();

        public Error Error { get; } = Error.None;
    }
}
