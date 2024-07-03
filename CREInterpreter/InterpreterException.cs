using System;

namespace CREInterpreter;

public class InterpreterException(string? Message = null) : Exception(Message)
{
    public override string ToString() => Message ?? "InterpreterException";
}