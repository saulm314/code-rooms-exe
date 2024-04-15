using System.Collections.Generic;

namespace CRECSharpInterpreter
{
    public class MemoryFrame
    {
        public MemoryFrame(Statement statement)
        {
            Statement = statement;
            Stack = Copy(Memory.Instance!.Stack);
            Heap = Copy(Memory.Instance.Heap);
        }

        public Statement Statement { get; init; }
        public Stack<Scope> Stack { get; init; } = new();
        public Heap Heap { get; init; }

        private Variable Copy(Variable oldVariable)
        {
            Variable variable = new(oldVariable._VarType, oldVariable.Name)
            {
                Value = oldVariable.Value
            };
            return variable;
        }

        private List<Variable> Copy(List<Variable> oldVariables)
        {
            List<Variable> variables = new(oldVariables.Count);
            foreach (Variable oldVariable in oldVariables)
            {
                Variable variable = Copy(oldVariable);
                variables.Add(variable);
            }
            return variables;
        }

        private Scope Copy(Scope oldScope)
        {
            List<Variable> oldVariables = oldScope.DeclaredVariables;
            List<Variable> variables = Copy(oldVariables);
            Scope scope = new()
            {
                DeclaredVariables = variables
            };
            return scope;
        }

        private Stack<Scope> Copy(Stack<Scope> oldStack)
        {
            Scope[] oldScopes = oldStack.ToArray();
            Stack<Scope> stack = new();
            for (int i = oldScopes.Length - 1; i >= 0; i--)
            {
                Scope oldScope = oldScopes[i];
                Scope scope = Copy(oldScope);
                stack.Push(scope);
            }
            return stack;
        }

        private Heap Copy(Heap oldHeap)
        {
            Heap heap = new();
            for (int i = 0; i < oldHeap.Size; i++)
            {
                Variable? oldVariable = oldHeap[i];
                Variable? variable = oldVariable == null ? null : Copy(oldVariable);
                heap[i] = variable;
            }
            return heap;
        }

        public override string ToString()
        {
            return Statement.ToString();
        }
    }
}
