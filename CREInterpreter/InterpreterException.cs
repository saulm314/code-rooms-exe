namespace CREInterpreter;

using System;

public class InterpreterException(string? Message = null) : Exception(Message)
{
    public override string ToString() => Message ?? "InterpreterException";
}