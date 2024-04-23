using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.Good
{
    public class VarDeclInit : ITest
    {
        public VarDeclInit(string pathNoExt)
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
                    new(@double, "_myDouble", 1.0, true),
                    new(@bool, "myB0", true, true),
                    new(@char, "myChar", 'a', true),
                    new(@int.Array, "intArr", 1, true),
                    new(@double.Array, "doubleArr", 2, true),
                    new(@bool.Array, "bArr", 3, true),
                    new(@char.Array, "charArr", 4, true),
                    new(@string, "myStr", 5, true),
                    new(@string.Array, "strArr", 6, true),
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
