namespace CREInterpreter.Statements;

public readonly struct StatementExecution(IStatement statement, InterpreterException? error)
{
    public IStatement Statement => statement;
    public InterpreterException? Error => error;
}