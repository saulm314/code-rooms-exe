namespace CREInterpreter.Tokens;

public class TypeNameToken(string text, VarType varType, int lineNumber) : IToken
{
    public string Text => text;

    public int LineNumber => lineNumber;

    public VarType _VarType => varType;
}