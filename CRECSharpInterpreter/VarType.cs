using System;
using System.Collections.Generic;

namespace CRECSharpInterpreter
{
    public class VarType
    {
        public string Name { get; init; }
        public Type SystemType { get; init; }
        public bool IsArray { get; init; }
        
        // the respective array of the type if it is not an array, else null
        public VarType? Array { get; init; }

        // the respective normal type if it is an array, else null
        public VarType? Unarray { get; init; }

        public object? DefaultValue
        {
            get => Name switch
                {
                    "int" => default(int),
                    "bool" => default(bool),
                    "char" => default(char),
                    "double" => default(double),
                    _ => null
                };
        }

        public Storage _Storage { get => _storage ??= DefaultValue != null ? Storage.Value : Storage.Reference; }
        private Storage? _storage;

        private VarType(string name, Type systemType, bool isArray = false, VarType? unarray = null)
        {
            Name = name;
            SystemType = systemType;
            IsArray = isArray;
            Unarray = unarray;

            VarTypes.Add(this);

            if (!isArray)
                Array = new(name + "[]", systemType.MakeArrayType(), true, this);
        }

        // must be declared above the types themselves,
        //      else a runtime error will occur during static construction
        public static List<VarType> VarTypes { get; } = new();

        public static VarType @int { get; } = new("int", typeof(int));
        public static VarType @bool { get; } = new("bool", typeof(bool));
        public static VarType @char { get; } = new("char", typeof(char));
        public static VarType @double { get; } = new("double", typeof(double));
        public static VarType @string { get; } = new("string", typeof(string));

        public static VarType? GetVarType(string varTypeAsString)
        {
            foreach (VarType varType in VarTypes)
                if (varType.Name == varTypeAsString)
                    return varType;
            return null;
        }

        public static VarType? GetVarType(Type varTypeAsSystemType)
        {
            foreach (VarType varType in VarTypes)
                if (varType.SystemType == varTypeAsSystemType)
                    return varType;
            return null;
        }

        public static VarType? GetVarType(object value)
        {
            Type type = value.GetType();
            return GetVarType(type);
        }

        public override string ToString() => Name;

        public enum Storage
        {
            Value,
            Reference
        }

        public class VarTypeException : InterpreterException
        {
            public VarTypeException(VarType? varType, string? message = null) : base(message)
            {
                this.varType = varType;
            }

            public VarType? varType;
        }
    }
}
