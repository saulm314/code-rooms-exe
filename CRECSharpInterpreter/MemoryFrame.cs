using System;
using System.Collections.Generic;

namespace CRECSharpInterpreter
{
    public class MemoryFrame
    {
        public MemoryFrame(Statement? statement = null)
        {
            Statement = statement;
            if (Memory.Instance != null)
                Memory.Instance.CurrentFrame++;
        }

        public void Init()
        {
            Stack = Copy(Memory.Instance!.Stack);
            ActiveLoops = Copy(Memory.Instance.ActiveLoops);
            ActiveForHeaders = Copy(Memory.Instance.ActiveForHeaders);
            Heap = Copy(Memory.Instance.Heap);
        }

        public Statement? Statement { get; init; }
        public Stack<Scope>? Stack { get; private set; }
        public Stack<int>? ActiveLoops { get; private set; }
        public Stack<int>? ActiveForHeaders { get; private set; }
        public Heap? Heap { get; private set; }

        public int Index { get; } = Memory.Instance?.Frames.Count ?? 0;

        // this refers to the last frame of the frames that have been computed thus far
        // just because it was the last frame at some point, doesn't mean it will stay as such
        public bool IsLastFrame => Index == Memory.Instance!.Frames.Count - 1;

        public bool CanMoveLeft => Index > 0;
        public bool CanMoveRight => !(IsLastFrame && (Memory.Instance!.Executed || Memory.Instance.Thrown));

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
            Heap heap = new(oldHeap.Size);
            for (int i = 0; i < oldHeap.Size; i++)
            {
                Variable? oldVariable = oldHeap[i];
                Variable? variable = oldVariable == null ? null : Copy(oldVariable);
                heap[i] = variable;
            }
            return heap;
        }

        private Stack<T> Copy<T>(Stack<T> oldStack) where T : struct
        {
            T[] oldItems = oldStack.ToArray();
            Stack<T> stack = new();
            for (int i = oldItems.Length - 1; i >= 0; i--)
                stack.Push(oldItems[i]);
            return stack;
        }

        public override string ToString()
        {
            string str = Statement?.ToString() ?? string.Empty;
            if (IsLastFrame && Memory.Instance!.Executed)
                return str += "Execution finished";
            if (IsLastFrame && Memory.Instance!.Thrown)
                return str += "\n\n" + Memory.Instance.ThrownException!.Message;
            return str;
        }
    }
}
