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
                case Type.DeclarationInitialisation:
                    VarType varType = VarType.VarTypes.Find(vt => vt.Name == KeyStrings[0].Text);
                    string varName = KeyStrings[1].Text;
                    DeclaredVariable = new(varType, varName);
                    break;
            }
        }

        public string Text { get; init; }

        public KeyString[] KeyStrings { get; init; }

        public Type _Type { get; init; }

        // appropriate variable if type is Declaration or DeclarationInitialisation, else null
        public Variable DeclaredVariable { get; init; }

        private static string[] nonKeywordKeyStrings = new string[]
        {
            "="
        };

        private string[] GetKeyStrings()
        {
            // ensure that every key string is surrounded by at least one space on either side
            string parsedText = Text;
            foreach (string keyString in nonKeywordKeyStrings)
                parsedText = parsedText.Replace(keyString, $" {keyString} ");

            // remove all whitespace characters and return everything else, separated
            return
                parsedText
                .Split(default(char[]), StringSplitOptions.TrimEntries)
                .Where(str => !string.IsNullOrWhiteSpace(str))
                .ToArray();
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

        public class LineException : Exception
        {
            public LineException(Line line, string message = null) : base(message)
            {
                this.line = line;
            }

            public Line line;
        }
    }
}
