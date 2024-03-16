namespace CRECSharpInterpreter
{
    public class KeyString
    {
        public KeyString(string text)
        {
            Text = text;
            _Type = GetType();

            VarType varType;
            switch (_Type)
            {
                case Type.Invalid:
                    throw new KeyStringException(this, $"Unrecognised key string: {Text}");
                case Type.Integer:
                    varType = VarType.@int;
                    int intValue = int.Parse(Text);
                    _Literal = new(varType, intValue);
                    break;
                case Type.Boolean:
                    varType = VarType.@bool;
                    bool boolValue = bool.Parse(Text);
                    _Literal = new(varType, boolValue);
                    break;
                case Type.Character:
                    varType = VarType.@char;
                    string substring = Text.Substring(1, Text.Length - 2);
                    char charValue;
                    try
                    {
                        charValue = char.Parse(substring);
                    }
                    catch
                    {
                        charValue = CharUtils.BasicEscapeCharacters[substring];
                    }
                    _Literal = new(varType, charValue);
                    break;
                case Type.DoubleFloat:
                    varType = VarType.@double;
                    double doubleValue = double.Parse(Text);
                    _Literal = new(varType, doubleValue);
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
            if (IsBoolean)
                return Type.Boolean;
            if (IsCharacter)
                return Type.Character;
            if (IsDoubleFloat)
                return Type.DoubleFloat;
            return Type.Invalid;
        }

        private bool IsKeyword { get => IsType || IsBoolean; }

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

        private bool IsBoolean { get => _isBoolean ??= Text == "false" || Text == "true"; }
        private bool? _isBoolean;

        private bool IsCharacter
        {
            get
            {
                if (_isCharacter != null)
                    return (bool)_isCharacter;
                if (Text[0] != '\'')
                    return _isCharacter ??= false;
                if (Text[Text.Length - 1] != '\'')
                    return _isCharacter ??= false;
                string substring = Text.Substring(1, Text.Length - 2);
                if (char.TryParse(substring, out _))
                    return _isCharacter ??= true;
                if (CharUtils.BasicEscapeCharacters.ContainsKey(substring))
                    return _isCharacter ??= true;
                return _isCharacter ??= false;
            }
        }
        private bool? _isCharacter;

        private bool IsDoubleFloat { get => _isDoubleFloat ??= double.TryParse(Text, out _); }
        private bool? _isDoubleFloat;

        public enum Type
        {
            Invalid,
            Type,
            Variable,
            Equals,
            Integer,
            Boolean,
            Character,
            DoubleFloat
        }

        public class KeyStringException : InterpreterException
        {
            public KeyStringException(KeyString keyString, string message = null) : base(message)
            {
                this.keyString = keyString;
            }

            KeyString keyString;
        }
    }
}
