using System;
using System.Linq;

namespace CRECSharpInterpreter
{
    public class Line
    {
        public Line(string text)
        {
            Text = text;
            string[] keyStringsStr = KeyString.GetKeyStringsAsStrings(Text);
            KeyStrings = new KeyString[keyStringsStr.Length];
            for (int i = 0; i < KeyStrings.Length; i++)
                KeyStrings[i] = new(keyStringsStr[i]);

            _Type = GetType();
            switch (_Type)
            {
                case Type.Invalid:
                    throw new LineException(this, $"Unrecognised operation in line:\n{Text}");
                case Type.Declaration:
                    VerifyDeclarationValid();
                    DeclareVariable();
                    break;
                case Type.DeclarationInitialisation:
                    VerifyDeclarationValid();
                    _Expression = CreateExpression(); // must be done before declaring variable to ensure no self-references
                    VarToWrite = DeclareVariable();
                    VerifyWriteValid();
                    VarToWrite.Initialised = true;
                    break;
                case Type.Write:
                    _Expression = CreateExpression();
                    VarToWrite = GetVarToWrite_NoDeclaration();
                    VerifyWriteValid();
                    VarToWrite.Initialised = true;
                    break;
            }

            if (Memory.Instance._Mode == Mode.Runtime)
                InitialiseRuntime();
        }

        public string Text { get; init; }

        public KeyString[] KeyStrings { get; init; }

        public Type _Type { get; init; }

        public Variable VarToWrite { get; init; }

        public Expression _Expression { get; init; }

        public static string[] GetLinesAsStrings(string text)
            =>
                text
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Where(str => !string.IsNullOrWhiteSpace(str))
                .ToArray();

        private void InitialiseRuntime()
        {
            string separator = Interpreter.SEPARATOR;
            Console.WriteLine(Text.TrimStart() + '\n');
            switch (_Type)
            {
                case Type.DeclarationInitialisation:
                case Type.Write:
                    PerformWrite();
                    break;
            }
            Console.WriteLine("Stack:");
            Console.WriteLine(separator + "\n");
            foreach (Scope scope in Memory.Instance.Stack)
            {
                foreach (Variable variable in scope.DeclaredVariables)
                    Console.WriteLine(variable);
                Console.WriteLine(separator);
            }
            Console.WriteLine("\nHeap:\n");
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                    Console.Write(string.Format("{0,10}", Memory.Instance.Heap[10 * i + j]?.ValueAsString ?? "x"));
                Console.Write("\n");
            }
            Console.WriteLine();
            Console.WriteLine(separator + separator + separator);
        }

        private void VerifyDeclarationValid()
        {
            string varName = KeyStrings[1].Text;
            if (Memory.Instance.IsDeclared(varName))
                throw new LineException(this, $"Variable {varName} has already been declared");
        }

        private Variable DeclareVariable()
        {
            VarType varType = VarType.VarTypes.Find(vt => vt.Name == KeyStrings[0].Text);
            string varName = KeyStrings[1].Text;
            Variable declaredVariable = new(varType, varName);
            Memory.Instance.AddToCurrentScope(declaredVariable);
            return declaredVariable;
        }

        private Expression CreateExpression()
        {
            int expressionOffset = _Type switch
            {
                Type.DeclarationInitialisation => 3,
                Type.Write => 2,
                _ => throw new LineException(this, "internal error")
            };
            KeyString[] expressionKeyStrings = new KeyString[KeyStrings.Length - expressionOffset];
            Array.Copy(KeyStrings, expressionOffset, expressionKeyStrings, 0, expressionKeyStrings.Length);
            return new(expressionKeyStrings);
        }

        private void VerifyWriteValid()
        {
            if (_Expression._VarType == null)
            {
                if (VarToWrite._VarType.DefaultValue == null)
                    return;
                throw new LineException(this,
                    $"Cannot write null to the value type variable \"{VarToWrite.Name}\"");
            }
            if (VarToWrite._VarType != _Expression._VarType)
                throw new LineException(this,
                    $"Cannot write expression of type {_Expression._VarType} to variable {VarToWrite.Name} of type {VarToWrite._VarType}");
        }

        private Variable GetVarToWrite_NoDeclaration()
        {
            Variable varToWrite = Memory.Instance.GetVariable(KeyStrings[0].Text);
            if (varToWrite == null)
                throw new LineException(this, $"Variable {KeyStrings[0].Text} hasn't been declared");
            return varToWrite;
        }

        private void PerformWrite()
        {
            _Expression.Compute();
            VarToWrite.Value = _Expression.Value;
        }

        private new Type GetType()
        {
            if (IsDeclaration)
                return Type.Declaration;
            if (IsDeclarationInitialisation)
                return Type.DeclarationInitialisation;
            if (IsWrite)
                return Type.Write;
            return Type.Invalid;
        }

        private bool IsDeclaration
        {
            get =>
                _isDeclaration ??=
                    KeyStrings.Length == 2 &&
                    KeyStrings[0]._Type == KeyString.Type.Type &&
                    KeyStrings[1]._Type == KeyString.Type.Variable;
        }
        private bool? _isDeclaration;

        private bool IsDeclarationInitialisation
        {
            get =>
                _isDeclarationInitialisation ??=
                    KeyStrings.Length >= 4 &&
                    KeyStrings[0]._Type == KeyString.Type.Type &&
                    KeyStrings[1]._Type == KeyString.Type.Variable &&
                    KeyStrings[2]._Type == KeyString.Type.Equals;
        }
        private bool? _isDeclarationInitialisation;

        private bool IsWrite
        {
            get =>
                _isWrite ??=
                    KeyStrings.Length >= 3 &&
                    KeyStrings[0]._Type == KeyString.Type.Variable &&
                    KeyStrings[1]._Type == KeyString.Type.Equals;
        }
        private bool? _isWrite;

        public enum Type
        {
            Invalid,
            Declaration,
            DeclarationInitialisation,
            Write
        }

        public class LineException : InterpreterException
        {
            public LineException(Line line, string message = null) : base(message)
            {
                this.line = line;
            }

            public Line line;
        }
    }
}
