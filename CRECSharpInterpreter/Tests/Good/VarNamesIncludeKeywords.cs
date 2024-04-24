using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.Good
{
    public class VarNamesIncludeKeywords : ITest
    {
        public VarNamesIncludeKeywords(string pathNoExt)
        {
            PathNoExt = pathNoExt;
        }

        public string PathNoExt { get; init; }

        public Variable[][] Stack => 
            new[]
            {
                new Variable[]
                {
                    new(@int, "if_", 1, true),
                    new(@int, "else_", 1, true),
                    new(@int, "while_", 1, true),
                    new(@int, "for_", 1, true),
                    new(@int, "break_", 1, true),
                    new(@int, "continue_", 1, true)
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
