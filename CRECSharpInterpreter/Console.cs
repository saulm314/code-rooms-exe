namespace CRECSharpInterpreter
{
    public static class Console
    {
        public static void WriteLine(object obj)
        {
            if (Environment.Testing)
                return;
            System.Console.WriteLine(obj);
        }

        public static void WriteLine()
        {
            if (Environment.Testing)
                return;
            System.Console.WriteLine();
        }

        public static void Write(object obj)
        {
            if (Environment.Testing)
                return;
            System.Console.Write(obj);
        }

        public static string? ReadLine()
        {
            if (Environment.Testing)
                return null;
            return System.Console.ReadLine();
        }
    }
}
