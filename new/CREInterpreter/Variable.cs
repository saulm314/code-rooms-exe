using System.Collections.Generic;

namespace CREInterpreter;

public class Variable(VarType? varType, string? name = null)
{
    public VarType? _VarType { get; } = varType;
    public string? Name { get; } = name;
    public object? Value { get; set; } = varType?.DefaultValue;
    public bool Initialised { get; set; }

    public string FormattedValue => ValueFormatter.FormatValue(Value, _VarType);

    public override string ToString() => string.Format("{0,10} {1,20} {2,10}",
            _VarType?.ToString() ?? "*",
            Name?.ToString() ?? "*",
            string.IsNullOrEmpty(FormattedValue) ? "*" : FormattedValue);
}