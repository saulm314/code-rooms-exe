using System;

namespace CRECSharpInterpreter
{
    public class Variable
    {
        public Variable(Type type, string name)
        {
            _Type = type;
            Name = name;
        }

        public Type _Type { get; set; }
        public string Name { get; set; }
        public object Value { get; set; }

        public enum Type
        {
            Invalid,
            @int
        }

        public static Type GetTypeFromString(string typeStr)
        {
            return typeStr switch
            {
                "int" => Type.@int,
                _ => throw new VariableException()
            };
        }

        public class VariableException : Exception
        {
            public VariableException(string message = null) : base(message) { }
        }
    }
}
