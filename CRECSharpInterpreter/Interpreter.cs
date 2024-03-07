namespace CRECSharpInterpreter
{
    public class Interpreter
    {
        public Interpreter(string text)
        {
            this.text = text;
            Console.WriteLine($"Interpreter for the following text created:\n{text}");
        }

        public readonly string text;

        public void Initialise()
        {
            throw new NotImplementedException();
        }

        public bool InterpretNextLine()
        {
            throw new NotImplementedException();
        }
    }
}
