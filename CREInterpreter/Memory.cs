using System.Collections.Generic;

namespace CREInterpreter;

public class Memory
{
    public Memory(int initialHeapCapacity)
    {
        PushToStack();
        Heap = new(initialHeapCapacity);
    }

    public Stack<Scope> Stack { get; } = new();
    public Heap Heap { get; init; }

    public void PushToStack()
    {
        Stack.Push([]);
    }

    public void PopFromStack()
    {
        Scope scope = Stack.Pop();
        foreach (Variable variable in scope)
        {
            if (variable._VarType!._Storage == Storage.Value)
                continue;
            if (variable.Value == null)
                continue;
            // is non-null reference type
            int heapIndex = (int)variable.Value;
            Heap.DecrementReferenceCounter(heapIndex);
        }
    }

    public IEnumerable<Variable> GetDeclaredVariables()
    {
        foreach (Scope scope in Stack)
            foreach (Variable variable in scope)
                yield return variable;
    }

    public Variable? GetVariable(string name)
    {
        foreach (Variable variable in GetDeclaredVariables())
            if (variable.Name == name)
                return variable;
        return null;
    }

    public bool IsDeclared(string variableName)
    {
        foreach (Variable variable in GetDeclaredVariables())
            if (variable.Name == variableName)
                return true;
        return false;
    }

    public void AddToCurrentScope(Variable variable)
    {
        Scope scope = Stack.Peek();
        scope.Add(variable);
    }
}