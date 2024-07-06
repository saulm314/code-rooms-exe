﻿namespace CREInterpreter.Tokens;

public class OpenSquareBraceSymbolToken(int lineNumber) : IToken, ISymbol
{
    public string Text => "[";

    public int LineNumber => lineNumber;

    public InterpreterException? Compile(Memory memory)
    {
        return null;
    }
}