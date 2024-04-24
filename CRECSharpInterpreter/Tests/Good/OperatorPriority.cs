using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.Good
{
    public class OperatorPriority : ITest
    {
        public OperatorPriority(string pathNoExt)
        {
            PathNoExt = pathNoExt;
        }

        public string PathNoExt { get; init; }

        public Variable[][] Stack => 
            new[]
            {
                new Variable[]
                {
                    new(@int, "myInt", 17, true),
                    new(@int, "myInt2", 17, true),
                    new(@int, "myInt3", 8, true),
                    new(@int, "myInt4", 8, true),
                    new(@int, "myInt5", 8, true),
                    new(@int, "myInt6", 8, true),
                    new(@int, "myInt7", 13, true),
                    new(@int, "myInt8", 5, true),
                    new(@bool, "myB", false, true),
                    new(@bool, "myB2", false, true),
                    new(@bool, "myB3", true, true),
                    new(@bool, "myB4", false, true),
                    new(@bool, "myB5", true, true),
                    new(@bool, "myB6", true, true),
                    new(@bool, "myB7", false, true),
                    new(@bool, "myB8", false, true),
                    new(@bool, "myB9", false, true),
                    new(@bool, "myB10", true, true),
                    new(@int, "myInt9", 8, true),
                    new(@int, "myInt10", 2, true),
                    new(@int, "myInt11", 2, true),
                    new(@int, "myInt12", 8, true),
                    new(@bool, "myB11", true, true),
                    new(@int, "myInt13", 8, true)
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
