namespace CREInterpreter.Statements;

public readonly record struct StatementExecution(IStatement Statement, InterpreterException? Exception);