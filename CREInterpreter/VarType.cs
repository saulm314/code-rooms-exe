using System;
using System.Collections.Generic;

namespace CREInterpreter;

public class VarType
{
    public string Name => SystemType.Name switch
    {
        "Int32" => "int",
        "Double" => "double",
        "Boolean" => "bool",
        "Char" => "char",
        "String" => "string",
        _ => throw new InterpreterException($"Internal error: cannot find name for systemtype {SystemType.Name}")
    };

    public Type SystemType { get; init; }
    public bool IsArray { get; init; }
    public VarType? Array { get; init; }
    public VarType? Unarray { get; init; }

    public object? DefaultValue => Name switch
    {
        "int" => default(int),
        "double" => default(double),
        "bool" => default(bool),
        "char" => default(char),
        _ => null
    };

    public Storage _Storage => _storage ??= DefaultValue != null ? Storage.Value : Storage.Reference;
    private Storage? _storage;

    private VarType(Type systemType, VarType? unarray = null)
    {
        SystemType = systemType;
        IsArray = unarray != null;
        Unarray = unarray;

        Array = !IsArray ? new(SystemType.MakeArrayType(), this) : null;

        VarTypes.Add(this);
    }

    public static List<VarType> VarTypes { get; } = [];

    public static VarType @int { get; } = new(typeof(int));
    public static VarType @bool { get; } = new(typeof(bool));
    public static VarType @char { get; } = new(typeof(char));
    public static VarType @double { get; } = new(typeof(double));
    public static VarType @string { get; } = new(typeof(string));

    public static VarType? GetVarType(string name) => VarTypes.Find(varType => varType.Name == name);
    public static VarType? GetVarType(Type systemType) => VarTypes.Find(varType => varType.SystemType == systemType);
    public static VarType? GetVarType(object value) => VarTypes.Find(varType => varType.SystemType == value.GetType());

    public override string ToString() => Name;
}