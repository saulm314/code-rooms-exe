using System;
using System.Collections.Generic;

namespace CRECSharpInterpreter
{
    public class VarType
    {
        public string Name { get; init; }
        public Type SystemType { get; init; }
        public bool IsArray { get; init; }
        public VarType Array { get; init; }
        public object DefaultValue
        {
            get =>
                _defaultValue ??=
                    Name switch
                    {
                        "int" => default(int),
                        "bool" => default(bool),
                        "char" => default(char),
                        "double" => default(double),
                        _ => null
                    };
        }
        private object _defaultValue;

        private VarType(string name, Type systemType, bool isArray = false)
        {
            Name = name;
            SystemType = systemType;
            IsArray = isArray;

            VarTypes.Add(this);

            if (!isArray)
                Array = new(name + "[]", systemType.MakeArrayType(), true);
        }

        // must be declared above the types themselves,
        //      else a runtime error will occur during static construction
        public static List<VarType> VarTypes { get; } = new();

        public static VarType @int { get; } = new("int", typeof(int));
        public static VarType @bool { get; } = new("bool", typeof(bool));
        public static VarType @char { get; } = new("char", typeof(char));
        public static VarType @double { get; } = new("double", typeof(double));

        public static VarType GetVarType(string varTypeAsString)
        {
            foreach (VarType varType in VarTypes)
                if (varType.Name == varTypeAsString)
                    return varType;
            throw new VarTypeException(null, $"Could not find varType {varTypeAsString}");
        }

        public override string ToString() => Name;

        public class VarTypeException : InterpreterException
        {
            public VarTypeException(VarType varType, string message = null) : base(message)
            {
                this.varType = varType;
            }

            public VarType varType;
        }
    }
}
