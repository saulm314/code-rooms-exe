﻿namespace CREInterpreter.Tokens;

public class XorSymbolToken(int lineNumber) : IToken
{
    public string Text => "^";

    public int LineNumber => lineNumber;

    public InterpreterException? Compile(Memory memory)
    {
        throw new System.NotImplementedException();
    }
}