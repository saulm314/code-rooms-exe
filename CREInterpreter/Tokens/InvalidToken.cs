using System;

namespace CREInterpreter.Tokens;

public class InvalidToken(ReadOnlyMemory<char> text, int lineNumber, int index, InterpreterException exception) : IToken
{
    public ReadOnlyMemory<char> Text => text;

    public int LineNumber => lineNumber;

    public int Index => index;

    public InterpreterException Exception => exception;
}