namespace CRECSharpInterpreter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string dummyText = "Console.WriteLine(\"Hello World!\");\nConsole.WriteLine(\"Goodbye!\");";
            new Interpreter(dummyText);
            while (true) { }
        }
    }
}
