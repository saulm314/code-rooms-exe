namespace CRECSharpInterpreter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string dummyText = "int number;\nnumber = 1;";
            Interpreter interpreter = new(dummyText);
            interpreter.Initialise();
            while (true) { }
        }
    }
}
