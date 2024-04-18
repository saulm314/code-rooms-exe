namespace CRECSharpInterpreter
{
    public static class Environment
    {
        public static Syntax _Syntax { get; set; } = Syntax.CSharp;
        public static bool Debug { get; set; } = false;
        public static bool Testing { get; set; } = false;
    }
}
