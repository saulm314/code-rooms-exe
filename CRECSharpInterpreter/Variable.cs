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

        public string ValueAsString
        {
            get
            {
                switch (_VarType.Name)
                {
                    case "int":
                        return Value.ToString();
                    case "bool":
                        if ((bool)Value)
                            return "true";
                        return "false";
                    default:
                        throw new VariableException(this,
                            $"Internal error: cannot convert value of type {_VarType} to string");
                }
            }
        }

        public override string ToString()
        {
            return $"{_VarType}\t{Name}\t:\t{ValueAsString}";
        }
    }
}
