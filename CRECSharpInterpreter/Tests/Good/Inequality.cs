using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.Good
{
    public class Inequality : ITest
    {
        public Inequality(string pathNoExt)
        {
            PathNoExt = pathNoExt;
        }

        public string PathNoExt { get; init; }

        public Variable[][] Stack => 
            new[]
            {
                new Variable[]
                {
                    new(@bool, "myB", false, true),
                    new(@bool, "myB2", true, true),
                    new(@bool, "myB3", false, true),
                    new(@bool, "myB4", true, true),
                    new(@bool, "myB5", false, true),
                    new(@bool, "myB6", true, true),
                    new(@bool, "myB7", false, true),
                    new(@bool, "myB8", true, true),
                    new(@int.Array, "intArr", 1, true),
                    new(@int.Array, "intArr2", 2, true),
                    new(@string, "myStr", 3, true),
                    new(@string, "myStr2", 4, true),
                    new(@string.Array, "strArr", 5, true),
                    new(@string.Array, "strArr2", 6, true),
                    new(@bool, "myB9", false, true),
                    new(@bool, "myB10", true, true),
                    new(@bool, "myB11", false, true),
                    new(@bool, "myB12", true, true),
                    new(@bool, "myB13", false, true),
                    new(@bool, "myB14", true, true),
                    new(@string, "myStr3", null, true),
                    new(@bool, "myB15", false, true),
                    new(@bool, "myB16", true, true),
                    new(@bool, "myB17", false, true)
                }
            };

        public Variable?[] Heap =>
            new Variable?[]
            {
                new(null),
                new(@int, 0),
                new(@int, 0),
                new(@int, 0),
                new(@int, 0),
                new(@int, 0),
                new(@int, 0)
            };

        public Error Error { get; } = Error.None;
    }
}
