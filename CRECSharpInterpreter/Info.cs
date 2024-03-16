using System.Collections.Generic;

namespace CRECSharpInterpreter
{
    public class Info
    {
        public Info(Mode mode)
        {
            Instance = this;
            _Mode = mode;
        }

        public static Info Instance { get; private set; }

        public List<Variable> DeclaredVariables { get; } = new();
        public List<object> ConstructedArrays { get; } = new();

        public Mode _Mode { get; }
    }
}
