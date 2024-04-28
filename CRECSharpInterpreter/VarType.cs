using System;
using System.Collections.Generic;

namespace CRECSharpInterpreter
{
    public class VarType
    {
        public string Name => SystemType.Name switch
        {
            "Int32" => "int",
            "Double" => "double",
            "Boolean" when Environment._Syntax == Syntax.CSharp => "bool",
            "Boolean" when Environment._Syntax == Syntax.Java => "boolean",
            "Char" => "char",
            "Int32[]" => "int[]",
            "Double[]" => "double[]",
            "Boolean[]" when Environment._Syntax == Syntax.CSharp => "bool[]",
            "Boolean[]" when Environment._Syntax == Syntax.Java => "boolean[]",
            "Char[]" => "char[]",
            "String" when Environment._Syntax == Syntax.CSharp => "string",
            "String" when Environment._Syntax == Syntax.Java => "String",
            "String[]" when Environment._Syntax == Syntax.CSharp => "string[]",
            "String[]" when Environment._Syntax == Syntax.Java => "String[]",
            _ => throw new VarTypeException(this, "internal error")
        };

        public string Slug => _slug ??= SystemType.Name switch
        {
            "Int32" => "int",
            "Double" => "double",
            "Boolean" => "bool",
            "Char" => "char",
            "Int32[]" => "int[]",
            "Double[]" => "double[]",
            "Boolean[]" => "bool[]",
            "Char[]" => "char[]",
            "String" => "string",
            "String[]" => "string[]",
            _ => throw new VarTypeException(this, "internal error")
        };
        private string? _slug;

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
                    "bool" when Environment._Syntax == Syntax.CSharp => default(bool),
                    "boolean" when Environment._Syntax == Syntax.Java => default(bool),
                    "char" => default(char),
                    "double" => default(double),
                    _ => null
                };
        }

        public Storage _Storage { get => _storage ??= DefaultValue != null ? Storage.Value : Storage.Reference; }
        private Storage? _storage;

        private VarType(Type systemType, bool isArray = false, VarType? unarray = null)
        {
            SystemType = systemType;
            IsArray = isArray;
            Unarray = unarray;

            VarTypes.Add(this);

            if (!isArray)
                Array = new(systemType.MakeArrayType(), true, this);
        }

        // must be declared above the types themselves,
        //      else a runtime error will occur during static construction
        public static List<VarType> VarTypes { get; } = new();

        public static VarType @int { get; } = new(typeof(int));
        public static VarType @bool { get; } = new(typeof(bool));
        public static VarType @char { get; } = new(typeof(char));
        public static VarType @double { get; } = new(typeof(double));
        public static VarType @string { get; } = new(typeof(string));

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
