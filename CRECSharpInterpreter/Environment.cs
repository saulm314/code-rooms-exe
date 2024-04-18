namespace CRECSharpInterpreter
{
    public static class Environment
    {
        public static Syntax _Syntax { get; set; } = Syntax.CSharp;
        public static Debug _Debug { get; set; } = Debug.No;
    }
}
