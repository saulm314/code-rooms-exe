using System.Collections.Generic;

namespace CREInterpreter;

public static class BlankEnumerables
{
    public static IEnumerable<Variable> GetBlankVariables(VarType varType, int count)
    {
        for (int i = 0; i < count; i++)
            yield return new(varType);
    }

    public static IEnumerable<Variable?> GetNullVariables(int count)
    {
        for (int i = 0; i < count; i++)
            yield return null;
    }

    public static IEnumerable<VariableContainer> GetBlankVariableContainers(int count)
    {
        for (int i = 0; i < count; i++)
            yield return new();
    }
}