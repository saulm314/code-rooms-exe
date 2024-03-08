namespace CRECSharpInterpreter
{
    public class Variable
    {
        public Variable(string name)
        {
            Name = name;
        }

        public Type? _Type { get; set; }
        public string Name { get; set; }
        public object Value { get; set; }

        public enum Type
        {
            @int
        }
    }
}
