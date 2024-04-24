using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.Good
{
    public class Double : ITest
    {
        public Double(string pathNoExt)
        {
            PathNoExt = pathNoExt;
        }

        public string PathNoExt { get; init; }

        public Variable[][] Stack => 
            new[]
            {
                new Variable[]
                {
                    new(@double, "myDouble", 0.0, false),
                    new(@double, "myDouble2", 0.0, true),
                    new(@double, "myDouble3", -15.0, true),
                    new(@double, "myDouble4", 3.14, true),
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
