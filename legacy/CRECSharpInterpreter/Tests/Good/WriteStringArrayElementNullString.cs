using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.Good
{
    public class WriteStringArrayElementNullString : ITest
    {
        public WriteStringArrayElementNullString(string pathNoExt)
        {
            PathNoExt = pathNoExt;
        }

        public string PathNoExt { get; init; }

        public Variable[][] Stack => 
            new[]
            {
                new Variable[]
                {
                    new(@string.Array, "strArr", 2, true),
                    new(@string, "myStr", null, true)
                }
            };

        public Variable?[] Heap =>
            new Variable?[]
            {
                new(null),
                null,
                new(@int, 1),
                new(@string, null)
            };

        public Error Error { get; } = Error.None;
    }
}
