namespace CREInterpreter;

using System;

public class InterpreterException(string? message = null) : Exception(message) { }