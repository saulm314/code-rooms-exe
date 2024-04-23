using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.Good
{
    public class VarDeclaration : ITest
    {
        public VarDeclaration(string pathNoExt)
        {
            PathNoExt = pathNoExt;
        }

        public string PathNoExt { get; init; }

        public Variable[][] Stack => 
            new[]
            {
                new Variable[]
                {
                    new(@int, "myInt", 0, false),
                    new(@double, "_myDouble", 0.0, false),
                    new(@bool, "myB0", false, false),
                    new(@char, "myChar_", '\0', false),
                    new(@int.Array, "intArr", null, false),
                    new(@double.Array, "doubleArr", null, false),
                    new(@bool.Array, "bArr", null, false),
                    new(@char.Array, "charArr", null, false),
                    new(@string, "myStr", null, false),
                    new(@string.Array, "strArr", null, false)
                }
            };

        public Variable[] Heap =>
            new Variable[]
            {
                new(null)
            };

        public Error Error { get; } = Error.None;
    }
}
