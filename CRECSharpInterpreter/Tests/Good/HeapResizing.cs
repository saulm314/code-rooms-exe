using static CRECSharpInterpreter.VarType;

namespace CRECSharpInterpreter.Tests.Good
{
    public class HeapResizing : ITest
    {
        public HeapResizing(string pathNoExt)
        {
            PathNoExt = pathNoExt;
        }

        public string PathNoExt { get; init; }

        public Variable[][] Stack => 
            new[]
            {
                new Variable[]
                {
                    new(@char.Array, "charArr", 1, true)
                }
            };

        public Variable[] Heap
        {
            get
            {
                Variable[] variables = new Variable[212];
                variables[0] = new(null);
                variables[1] = new(@int, 210);
                for (int i = 2; i < 212; i++)
                    variables[i] = new(@char, '\0');
                return variables;
            }
        }

        public Error Error { get; } = Error.None;
    }
}
