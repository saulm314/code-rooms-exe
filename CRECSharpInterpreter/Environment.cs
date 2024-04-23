using System;

namespace CRECSharpInterpreter
{
    public static class Environment
    {
        public static Syntax _Syntax { get; set; } = Syntax.CSharp;
        public static bool Debug { get; set; } = false;
        public static bool Testing { get; set; } = false;
        public static bool Verbose { get; set; } = false;
        public static string[]? Tests { get; set; }
        public static bool ShouldPrint { get; set; } = true;
    }
}
