using System.Collections.Generic;

namespace CRECSharpInterpreter
{
    public class Scope
    {
        public Scope() { }
        internal Scope(List<Variable> variables)
        {
            DeclaredVariables = variables;
        }

        public List<Variable> DeclaredVariables { get; internal set; } = new();
    }
}
