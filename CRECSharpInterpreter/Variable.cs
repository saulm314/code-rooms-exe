using System;

namespace CRECSharpInterpreter
{
    public class Variable
    {
        public Variable(VarType varType, string name)
        {
            _VarType = varType;
            Name = name;
        }

        public VarType _VarType { get; init; }
        public string Name { get; init; }
        public object Value { get; set; }
        public bool Initialised { get; set; }

        public class VariableException : InterpreterException
        {
            public VariableException(Variable variable, string message = null) : base(message)
            {
                this.variable = variable;
            }

            public Variable variable;
        }
    }
}
