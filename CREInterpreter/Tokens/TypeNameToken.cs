namespace CREInterpreter.Tokens;

public class TypeNameToken(string text, VarType varType, int lineNumber, int index) : IToken
{
    public string Text => text;

    public int LineNumber => lineNumber;

    public int Index => index;

    public VarType _VarType => varType;
}