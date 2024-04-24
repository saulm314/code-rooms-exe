using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.Good
{
    public class WriteStringNullString : ITest
    {
        public WriteStringNullString(string pathNoExt)
        {
            PathNoExt = pathNoExt;
        }

        public string PathNoExt { get; init; }

        public Variable[][] Stack => 
            new[]
            {
                new Variable[]
                {
                    new(@string, "myStr", null, true),
                    new(@string, "myStr2", null, true)
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
