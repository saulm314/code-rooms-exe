using System.Collections.Generic;

namespace CRECSharpInterpreter
{
    public class Scope
    {
        public List<Variable> DeclaredVariables { get; internal set; } = new();
    }
}
