namespace CREInterpreter;

public class Environment
{
    public static Environment Instance { get; } = new();

    protected Environment() { }

    public Syntax _Syntax { get; set; } = Syntax.CSharp;
}
