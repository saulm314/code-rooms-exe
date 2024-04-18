using System;
using System.Collections.Generic;

namespace CRECSharpInterpreter
{
    public class Memory
    {
        public Memory(Mode mode)
        {
            Instance = this;
            _Mode = mode;
            Stack.Push(new());
            if (_Mode == Mode.RuntimeStoreAllFrames)
                Frames.Add(new());
        }

        public static Memory? Instance { get; private set; }

        public Stack<int> ActiveLoops { get; } = new();

        public Stack<Scope> Stack { get; } = new();
        public Heap Heap { get; } = new();

        public List<MemoryFrame> Frames { get; } = new();

        public int CurrentFrame { get; internal set; } = -1;

        public bool Executed { get; internal set; } = false;
        public bool Thrown { get; internal set; } = false;
        public InterpreterException? ThrownException { get; internal set; }

        public void PushToStack()
        {
            Stack.Push(new());
        }

        public void PopFromStack()
        {
            Scope scope = Stack.Pop();
            foreach (Variable variable in scope.DeclaredVariables)
            {
                if (variable._VarType!._Storage == VarType.Storage.Value)
                    continue;
                if (variable.Value == null)
                    continue;
                // is non-null reference type
                int heapIndex = (int)variable.Value;
                Heap.DecrementReferenceCounter(heapIndex);
            }
        }

        public void PushLoopToStack()
        {
            ActiveLoops.Push(Stack.Count);
            PushToStack();
        }

        public void PopLoopFromStack()
        {
            int desiredStackCount;
            try
            {
                desiredStackCount = ActiveLoops.Pop();
            }
            catch (InvalidOperationException)
            {
                throw new MemoryException(this, "No loop to break from");
            }
            if (Stack.Count <= desiredStackCount)
                throw new MemoryException(this, "Internal error");
            while (Stack.Count > desiredStackCount)
                PopFromStack();
        }

        public void VerifyPopLoopValid()
        {
            if (ActiveLoops.Count == 0)
                throw new MemoryException(this, "No loop to break from");
        }

        public IEnumerable<Variable> GetDeclaredVariables()
        {
            foreach (Scope scope in Stack)
                foreach (Variable variable in scope.DeclaredVariables)
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
            scope.DeclaredVariables.Add(variable);
        }

        public Mode _Mode { get; }

        public class MemoryException : InterpreterException
        {
            public MemoryException(Memory? memory, string? message = null) : base(message)
            {
                this.memory = memory;
            }

            public Memory? memory;
        }
    }
}
