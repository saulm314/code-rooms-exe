using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.Good
{
    public class Negation : ITest
    {
        public Negation(string pathNoExt)
        {
            PathNoExt = pathNoExt;
        }

        public string PathNoExt { get; init; }

        public Variable[][] Stack => 
            new[]
            {
                new Variable[]
                {
                    new(@int, "myInt", -5, true),
                    new(@int, "myInt2", 5, true),
                    new(@int, "myInt3", -5, true),
                    new(@double, "myDouble", -5.0, true),
                    new(@double, "myDouble2", 5.0, true),
                    new(@double, "myDouble3", -5.0, true)
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
