using static CRECSharpInterpreter.VarType;
using System;

namespace CRECSharpInterpreter.Tests.Variables
{
    public class BadTest : ITest
    {
        public BadTest(string path, Error error)
        {
            Path = path;
            Error = error;
        }

        public string Path { get; init; }

        public Variable[][] Stack => Array.Empty<Variable[]>();

        public Variable[] Heap => Array.Empty<Variable>();

        public Error Error { get; init; }
    }
}
