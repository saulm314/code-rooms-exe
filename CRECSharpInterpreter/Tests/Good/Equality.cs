using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.Good
{
    public class Equality : ITest
    {
        public Equality(string pathNoExt)
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
                    new(@bool, "myB2", false, true),
                    new(@bool, "myB3", true, true),
                    new(@bool, "myB4", false, true),
                    new(@bool, "myB5", true, true),
                    new(@bool, "myB6", false, true),
                    new(@bool, "myB7", true, true),
                    new(@bool, "myB8", false, true),
                    new(@int.Array, "intArr", 1, true),
                    new(@int.Array, "intArr2", 2, true),
                    new(@string, "myStr", 3, true),
                    new(@string, "myStr2", 4, true),
                    new(@string.Array, "strArr", 5, true),
                    new(@string.Array, "strArr2", 6, true),
                    new(@bool, "myB9", true, true),
                    new(@bool, "myB10", false, true),
                    new(@bool, "myB11", true, true),
                    new(@bool, "myB12", false, true),
                    new(@bool, "myB13", true, true),
                    new(@bool, "myB14", false, true),
                    new(@string, "myStr3", null, true),
                    new(@bool, "myB15", true, true),
                    new(@bool, "myB16", false, true),
                    new(@bool, "myB17", true, true)
                }
            };

        public Variable[] Heap =>
            new Variable[]
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
