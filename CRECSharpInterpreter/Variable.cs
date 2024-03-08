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

        public class VariableException : Exception
        {
            public VariableException(string message = null) : base(message) { }
        }
    }
}
