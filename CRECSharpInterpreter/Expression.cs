using System.Collections.Generic;

namespace CRECSharpInterpreter
{
    public class Expression
    {
        public Expression(KeyString[] keyStrings, List<Variable> declaredVariables)
        {
            KeyStrings = keyStrings;
            DeclaredVariables = declaredVariables;
        }

        public KeyString[] KeyStrings { get; init; }
        public List<Variable> DeclaredVariables { get; init; }

        public VarType _VarType { get; private set; }
        public object Value { get; private set; }

        public bool IsValid()
        {
            return false;
        }
    }
}
