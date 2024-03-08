using System;
using System.Linq;

namespace CRECSharpInterpreter
{
    public class KeyString
    {
        public KeyString(string text)
        {
            Text = text;
            _Type = GetType();

            switch (_Type)
            {
                case Type.Invalid:
                    throw new KeyStringException(this, $"Unrecognised key string: {Text}");
                case Type.Integer:
                    VarType varType = VarType.@int;
                    int value = int.Parse(Text);
                    _Literal = new(varType, value);
                    break;
            }
        }

        public string Text { get; init; }

        public Type _Type { get; init; }

        // null if not a literal
        public Literal _Literal { get; init; }

        private new Type GetType()
        {
            if (IsType)
                return Type.Type;
            if (IsVariable)
                return Type.Variable;
            if (IsEquals)
                return Type.Equals;
            if (IsInteger)
                return Type.Integer;
            return Type.Invalid;
        }

        private bool IsKeyword { get => IsType; }

        private bool IsType { get => _isType ??= VarType.VarTypes.Exists(varType => varType.Name == Text); }
        private bool? _isType;

        private bool IsVariable
        {
            get
            {
                if (_isVariable != null)
                    return (bool)_isVariable;
                if (Text.Length == 0)
                    return _isVariable ??= false;
                if (IsKeyword)
                    return _isVariable ??= false;
                if (!char.IsLetter(Text[0]) && Text[0] != '_')
                    return _isVariable ??= false;
                for (int i = 1; i < Text.Length; i++)
                    if (!char.IsLetterOrDigit(Text[i]) && Text[i] != '_')
                        return _isVariable ??= false;
                return _isVariable ??= true;
            }
        }
        private bool? _isVariable;

        private bool IsEquals { get => _isEquals ??= Text == "="; }
        private bool? _isEquals;

        private bool IsInteger { get => _isInteger ??= int.TryParse(Text, out _); }
        private bool? _isInteger;

        public enum Type
        {
            Invalid,
            Type,
            Variable,
            Equals,
            Integer
        }

        public class KeyStringException : Exception
        {
            public KeyStringException(KeyString keyString, string message = null) : base(message)
            {
                this.keyString = keyString;
            }

            KeyString keyString;
        }
    }
}
