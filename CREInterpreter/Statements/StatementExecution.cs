namespace CREInterpreter.Statements;

public readonly struct StatementExecution(IStatement statement, InterpreterException? exception)
{
    public IStatement Statement => statement;
    public InterpreterException? Exception => exception;
}