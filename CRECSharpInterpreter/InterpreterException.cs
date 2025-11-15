using System;

namespace CRECSharpInterpreter
{
    public class InterpreterException : Exception
    {
        public InterpreterException(string? message = null) : base(message) { }
    }
}
