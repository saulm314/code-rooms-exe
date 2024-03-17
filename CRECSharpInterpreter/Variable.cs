using System;
using System.Collections.Generic;

namespace CRECSharpInterpreter
{
    public class Variable
    {
        public Variable(VarType varType, string name = null)
        {
            _VarType = varType;
            Name = name;
            Value = varType.DefaultValue;
        }

        public VarType _VarType { get; init; }
        public string Name { get; init; }
        public object Value { get; set; }
        public bool Initialised { get; set; }

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
                    case "char":
                        if (!CharUtils.BasicEscapeCharacters.ContainsValue((char)Value))
                            return $"'{Value}'";
                        Dictionary<string, char>.KeyCollection keys = CharUtils.BasicEscapeCharacters.Keys;
                        foreach (string key in keys)
                        {
                            if (CharUtils.BasicEscapeCharacters[key] == (char)Value)
                                return $"'{key}'";
                        }
                        throw new VariableException(this,
                            $"Internal error: cannot convert character {Value} to string");
                    case "double":
                        return Value.ToString();
                    default:
                        if (_VarType.IsArray)
                            break;
                        throw new VariableException(this,
                            $"Internal error: cannot convert value of type {_VarType} to string");
                }

                // varType is an array
                if (Value == null)
                    return "null";
                return $"{{{Value}}}";
            }
        }

        public override string ToString()
        {
            return $"{_VarType}\t{Name}\t:\t{ValueAsString}";
        }

        public static IEnumerable<Variable> GetBlankVariables(VarType varType, int count)
        {
            for (int i = 0; i < count; i++)
                yield return new(varType);
        }

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
