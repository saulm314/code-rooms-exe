using Newtonsoft.Json;

namespace CRECSharpInterpreter.Levels
{
    public class Level
    {
        public int id;
        public string? name;
        public Variable[][]? initialStacks;
        public Variable[][][]? finalStacks;

        public override string ToString()
        {
            return $"{id} {name} {initialStacks!.Length} {finalStacks!.Length}";
        }
    }
}
