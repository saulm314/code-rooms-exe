using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.Good
{
    public class Cast : ITest
    {
        public Cast(string pathNoExt)
        {
            PathNoExt = pathNoExt;
        }

        public string PathNoExt { get; init; }

        public Variable[][] Stack => 
            new[]
            {
                new Variable[]
                {
                    new(@int, "myInt", 1, true),
                    new(@double, "myDouble", 1.0, true),
                    new(@bool, "myB", true, true),
                    new(@char, "myChar", 'a', true),
                    new(@int.Array, "intArr", 1, true),
                    new(@string, "myStr", 2, true),
                    new(@string.Array, "strArr", 3, true),
                    new(@int.Array, "intArr2", 1, true),
                    new(@string, "myStr2", 2, true),
                    new(@string.Array, "strArr2", 3, true),
                    new(@int.Array, "intArr3", null, true),
                    new(@string, "myStr3", null, true),
                    new(@string.Array, "strArr3", null, true),
                    new(@double, "myDouble2", 2.0, true),
                    new(@int, "myInt2", 2, true),
                    new(@int, "myInt3", 5, true),
                    new(@int, "myInt4", 5, true),
                    new(@int, "myInt5", 5, true)
                }
            };

        public Variable?[] Heap =>
            new Variable?[]
            {
                new(null),
                new(@int, 0),
                new(@int, 0),
                new(@int, 0)
            };

        public Error Error { get; } = Error.None;
    }
}
