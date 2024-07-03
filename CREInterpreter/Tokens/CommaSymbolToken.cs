﻿namespace CREInterpreter.Tokens;

public class CommaSymbolToken(int lineNumber) : IToken
{
    public string Text => ",";

    public int LineNumber => lineNumber;

    public InterpreterException? Compile(Memory memory)
    {
        throw new System.NotImplementedException();
    }
}