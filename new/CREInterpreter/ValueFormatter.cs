using System;
using System.Linq;

namespace CREInterpreter;

public static class ValueFormatter
{
    public static string FormatValue(object? value, VarType? varType)
    {
        if (value == null)
            return "null";
        if (varType == null)
            return $"{{{value}}}";
        if (value.GetType() != varType.SystemType)
            throw new ArgumentException($"Cannot format value of type {value.GetType()} with varType {varType}");
        return varType.Name switch
        {
            "int" => value.ToString()!,
            "bool" => (bool)value ? "true" : "false",
            "double" => (int)value == (double)value ? $"{value}.0" : value.ToString()!,
            "char" => !CharUtils.BasicEscapeCharacters.ContainsValue((char)value) ? $"'{value}'" :
                CharUtils.BasicEscapeCharacters.Keys.Single(key => CharUtils.BasicEscapeCharacters[key] == (char)value),
            _ => throw new InterpreterException($"Internal error when formatting value; unrecognised varType {varType}")
        };
    }
}