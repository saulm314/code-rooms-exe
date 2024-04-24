using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.Good
{
    public class ValueTypeArrayDeclInit : ITest
    {
        public ValueTypeArrayDeclInit(string pathNoExt)
        {
            PathNoExt = pathNoExt;
        }

        public string PathNoExt { get; init; }

        public Variable[][] Stack => 
            new[]
            {
                new Variable[]
                {
                    new(@int.Array, "intArr", null, false),
                    new(@double.Array, "doubleArr", null, false),
                    new(@bool.Array, "bArr", null, false),
                    new(@char.Array, "charArr", null, false),
                    new(@int.Array, "intArr2", null, true),
                    new(@double.Array, "doubleArr2", 1, true),
                    new(@bool.Array, "bArr2", 2, true),
                    new(@char.Array, "charArr2", 5, true)
                }
            };

        public Variable[] Heap =>
            new Variable[]
            {
                new(null),
                new(@int, 0),
                new(@int, 2),
                new(@bool, false),
                new(@bool, false),
                new(@int, 3),
                new(@char, 'a'),
                new(@char, 'b'),
                new(@char, 'c')
            };

        public Error Error { get; } = Error.None;
    }
}
