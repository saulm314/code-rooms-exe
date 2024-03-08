using System.Collections.Generic;

namespace CRECSharpInterpreter
{
    public class VarType
    {
        public string Name { get; init; }
        public System.Type SystemType { get; init; }

        private VarType(string name, System.Type systemType)
        {
            Name = name;
            SystemType = systemType;

            VarTypes.Add(this);
        }

        // must be declared above the types themselves,
        //      else a runtime error will occur during static construction
        public static List<VarType> VarTypes { get; } = new();

        public static VarType @int { get; } = new("int", typeof(int));

        public override string ToString() => Name;
    }
}
