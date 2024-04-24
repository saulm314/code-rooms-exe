using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.Good
{
    public class ReadWriteVar : ITest
    {
        public ReadWriteVar(string pathNoExt)
        {
            PathNoExt = pathNoExt;
        }

        public string PathNoExt { get; init; }

        public Variable[][] Stack => 
            new[]
            {
                new Variable[]
                {
                    new(@int, "myInt", 2, true),
                    new(@int, "myInt2", 2, true),
                    new(@int, "myInt3", 7, true),
                    new(@int, "myInt4", 7, true)
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
