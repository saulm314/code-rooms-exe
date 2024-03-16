using System;
using System.Linq;

namespace CRECSharpInterpreter
{
    public class Line
    {
        public Line(string text)
        {
            Text = text;
            string[] keyStringsStr = GetKeyStrings();
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

            if (Info.Instance._Mode == Mode.Runtime)
                InitialiseRuntime();
        }

        public string Text { get; init; }

        public KeyString[] KeyStrings { get; init; }

        public Type _Type { get; init; }

        public Variable VarToWrite { get; init; }

        public Expression _Expression { get; init; }

        private void InitialiseRuntime()
        {
            Console.WriteLine(Text.TrimStart() + '\n');
            Console.WriteLine("Stack:");
            switch (_Type)
            {
                case Type.DeclarationInitialisation:
                case Type.Write:
                    PerformWrite();
                    break;
            }
            foreach (Variable variable in Info.Instance.DeclaredVariables)
                Console.WriteLine(variable);
            Console.WriteLine("\nHeap:\n");
            for (int i = 0; i < Info.Instance.ConstructedArrays.Count; i++)
            {
                Console.WriteLine($"{{{i}}}:");
                Array array = Info.Instance.ConstructedArrays[i];
                VarType varType = VarType.GetVarType(array.GetType()).Unarray;
                foreach (object element in array)
                {
                    Variable variable = new(varType, null);
                    Console.WriteLine(variable.ValueAsString);
                }
                Console.WriteLine();
            }
            Console.WriteLine("__________________________________");
        }

        private void VerifyDeclarationValid()
        {
            string varName = KeyStrings[1].Text;
            if (Info.Instance.DeclaredVariables.Exists(var => var.Name == varName))
                throw new LineException(this, $"Variable {varName} has already been declared");
        }

        private Variable DeclareVariable()
        {
            VarType varType = VarType.VarTypes.Find(vt => vt.Name == KeyStrings[0].Text);
            string varName = KeyStrings[1].Text;
            Variable declaredVariable = new(varType, varName);
            Info.Instance.DeclaredVariables.Add(declaredVariable);
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

            // verify that no invalid variables are present
            // this leaves less work for the expression itself
            //      as it does not have to verify whether each variable has been declared
            foreach (KeyString keyString in expressionKeyStrings)
            {
                if (keyString._Type == KeyString.Type.Variable)
                {
                    Variable variable = Info.Instance.DeclaredVariables.Find(var => var.Name == keyString.Text);
                    if (variable == null)
                        throw new LineException(this, $"Variable {keyString.Text} hasn't been declared");
                    if (!variable.Initialised)
                        throw new LineException(this, $"Variable {keyString.Text} hasn't been initialised");
                }
            }
            return new(expressionKeyStrings);
        }

        private void VerifyWriteValid()
        {
            if (VarToWrite._VarType != _Expression._VarType)
                throw new LineException(this,
                    $"Cannot write expression of type {_Expression._VarType} to variable {VarToWrite.Name} of type {VarToWrite._VarType}");
        }

        private Variable GetVarToWrite_NoDeclaration()
        {
            Variable varToWrite = Info.Instance.DeclaredVariables.Find(var => var.Name == KeyStrings[0].Text);
            if (varToWrite == null)
                throw new LineException(this, $"Variable {KeyStrings[0].Text} hasn't been declared");
            return varToWrite;
        }

        private void PerformWrite()
        {
            _Expression.Compute();
            VarToWrite.Value = _Expression.Value;
        }

        private static string[] nonKeywordKeyStrings = new string[]
        {
            "=",
            "{",
            "}"
        };

        private string[] GetKeyStrings()
        {
            // ensure that every key string is surrounded by at least one space on either side
            string parsedText = Text;
            foreach (string keyString in nonKeywordKeyStrings)
                parsedText = parsedText.Replace(keyString, $" {keyString} ");

            parsedText = RemoveWhiteSpaceSurroundingCharacter(parsedText, '[', Direction.LeftRight);
            parsedText = RemoveWhiteSpaceSurroundingCharacter(parsedText, ']', Direction.Left);

            // remove all whitespace characters and return everything else, separated
            return
                parsedText
                .Split(default(char[]), StringSplitOptions.TrimEntries)
                .Where(str => !string.IsNullOrWhiteSpace(str))
                .ToArray();
        }

        private string RemoveWhiteSpaceSurroundingCharacter(string input, char character, Direction direction)
        {
            int i = 0;
            while (i < input.Length)
            {
                if (input[i] == character)
                    input = RemoveWhiteSpaceSurroundingIndex(input, i, direction, out i);
                i++;
            }
            return input;
        }

        private enum Direction
        {
            Left,
            Right,
            LeftRight
        }

        private string RemoveWhiteSpaceSurroundingIndex(string input, int index, Direction direction, out int newIndex)
        {
            newIndex = index;
            switch (direction)
            {
                case Direction.Left:
                    return RemoveWhiteSpaceLeftOfIndex(input, index, out newIndex);
                case Direction.Right:
                    return RemoveWhiteSpaceRightOfIndex(input, index);
                case Direction.LeftRight:
                    input = RemoveWhiteSpaceLeftOfIndex(input, index, out newIndex);
                    return RemoveWhiteSpaceRightOfIndex(input, newIndex);
                default:
                    throw new LineException(this,
                        $"Internal exception: Invalid direction {direction}");
            }
        }

        private string RemoveWhiteSpaceLeftOfIndex(string input, int index, out int newIndex)
        {
            newIndex = index;
            int i = newIndex - 1;
            while (i >= 0)
                if (char.IsWhiteSpace(input[i]))
                {
                    input = input.Remove(i, 1);
                    newIndex--;
                    i--;
                }
                else
                    break;
            return input;
        }

        private string RemoveWhiteSpaceRightOfIndex(string input, int index)
        {
            int i = index + 1;
            while (i < input.Length)
                if (char.IsWhiteSpace(input[i]))
                    input = input.Remove(i, 1);
                else
                    break;
            return input;
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
