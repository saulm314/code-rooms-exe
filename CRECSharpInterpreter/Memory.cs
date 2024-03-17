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
        }

        public static Memory Instance { get; private set; }

        public Stack<Scope> Stack { get; } = new();
        public Heap Heap { get; } = new();

        public IEnumerable<Variable> GetDeclaredVariables()
        {
            foreach (Scope scope in Stack)
                foreach (Variable variable in scope.DeclaredVariables)
                    yield return variable;
        }

        public Variable GetVariable(string name)
        {
            foreach (Variable variable in GetDeclaredVariables())
                if (variable.Name == name)
                    return variable;
            throw new MemoryException(this, $"Cannot find variable \"{name}\"");
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
            public MemoryException(Memory memory, string message = null) : base(message)
            {
                this.memory = memory;
            }

            public Memory memory;
        }
    }
}
