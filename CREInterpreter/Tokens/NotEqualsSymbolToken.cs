﻿namespace CREInterpreter.Tokens;

public class NotEqualsSymbolToken(int lineNumber) : IToken
{
    public string Text => "!=";

    public int LineNumber => lineNumber;

    public InterpreterException? Compile(Memory memory)
    {
        return null;
    }
}