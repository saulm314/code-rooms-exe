using System;
using System.Linq;
using CRECSharpInterpreter.Operators;

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
                case Type.Type:
                    if (!Text.EndsWith("[]"))
                        break;
                    SubKeyStrings = new KeyString[3];
                    SubKeyStrings[0] = new(Text.Split("[]", StringSplitOptions.RemoveEmptyEntries)[0]);
                    SubKeyStrings[1] = new("[");
                    SubKeyStrings[2] = new("]");
                    break;
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
                case Type.ArrayConstruction:
                    SubKeyStrings = new KeyString[3 + _ArrayConstruction.ArrayLengthExpression.KeyStrings.Length];
                    SubKeyStrings[0] = new(_ArrayConstruction._VarType.Unarray.Name);
                    SubKeyStrings[1] = new("[");
                    _ArrayConstruction.ArrayLengthExpression.KeyStrings.CopyTo(SubKeyStrings, 2);
                    SubKeyStrings[SubKeyStrings.Length - 1] = new("]");
                    break;
                case Type.ArrayElement:
                    SubKeyStrings = new KeyString[3 + _ArrayElement.IndexExpression.KeyStrings.Length];
                    SubKeyStrings[0] = new(_ArrayElement.Array.Name);
                    SubKeyStrings[1] = new("[");
                    _ArrayElement.IndexExpression.KeyStrings.CopyTo(SubKeyStrings, 2);
                    SubKeyStrings[SubKeyStrings.Length - 1] = new("]");
                    break;
                case Type.ArrayLength:
                    SubKeyStrings = new KeyString[3];
                    SubKeyStrings[0] = new(_ArrayLength.Array.Name);
                    SubKeyStrings[1] = new(".");
                    SubKeyStrings[2] = new("Length");
                    break;
                case Type.Operator:
                    _Operator = Operator.GetOperator(Text);
                    break;
            }
        }

        public string Text { get; init; }

        public Type _Type { get; init; }

        // null if not a literal
        public Literal _Literal { get; init; }

        // null if not an array construction
        public ArrayConstruction _ArrayConstruction { get => _arrayConstruction ??= CreateArrayConstruction(); }
        private ArrayConstruction _arrayConstruction;
        private bool arrayConstructionIsNull;

        private ArrayConstruction CreateArrayConstruction()
        {
            if (arrayConstructionIsNull)
                return null;
            arrayConstructionIsNull = true;
            int openSquareBraceIndex = Text.IndexOf('[');
            if (openSquareBraceIndex == -1)
                return null;
            int closeSquareBraceIndex = Text.IndexOf(']');
            if (closeSquareBraceIndex == -1)
                return null;
            if (closeSquareBraceIndex <= openSquareBraceIndex)
                return null;
            string varTypeAsString = Text[..openSquareBraceIndex] + "[]";
            VarType varType = VarType.GetVarType(varTypeAsString);
            if (varType == null)
                return null;
            string stringInsideBraces = Text[(openSquareBraceIndex + 1)..closeSquareBraceIndex];
            if (string.IsNullOrWhiteSpace(stringInsideBraces))
                return null;
            string stringAfterBraces = Text[(closeSquareBraceIndex + 1)..];
            if (!string.IsNullOrWhiteSpace(stringAfterBraces))
                return null;
            ArrayConstruction arrayConstruction = new(varType, stringInsideBraces);
            arrayConstructionIsNull = false;
            return arrayConstruction;
        }

        public ArrayElement _ArrayElement { get => _arrayElement ??= CreateArrayElement(); }
        private ArrayElement _arrayElement;
        private bool arrayElementIsNull;

        private ArrayElement CreateArrayElement()
        {
            if (arrayElementIsNull)
                return null;
            arrayElementIsNull = true;
            int openSquareBraceIndex = Text.IndexOf('[');
            if (openSquareBraceIndex == -1)
                return null;
            int closeSquareBraceIndex = Text.IndexOf(']');
            if (closeSquareBraceIndex == -1)
                return null;
            if (closeSquareBraceIndex <= openSquareBraceIndex)
                return null;
            string variableName = Text[..openSquareBraceIndex];
            Variable variable = Memory.Instance.GetVariable(variableName);
            if (variable == null)
                return null;
            if (!variable.Initialised)
                return null;
            if (!variable._VarType.IsArray)
                return null;
            string stringInsideBraces = Text[(openSquareBraceIndex + 1)..closeSquareBraceIndex];
            if (string.IsNullOrWhiteSpace(stringInsideBraces))
                return null;
            string stringAfterBraces = Text[(closeSquareBraceIndex + 1)..];
            if (!string.IsNullOrWhiteSpace(stringAfterBraces))
                return null;
            ArrayElement arrayElement = new(variable, stringInsideBraces);
            arrayElementIsNull = false;
            return arrayElement;
        }

        public ArrayLength _ArrayLength { get => _arrayLength ??= CreateArrayLength(); }
        private ArrayLength _arrayLength;
        private bool arrayLengthIsNull;

        private ArrayLength CreateArrayLength()
        {
            if (arrayLengthIsNull)
                return null;
            arrayLengthIsNull = true;
            int dotIndex = Text.IndexOf('.');
            if (dotIndex == -1)
                return null;
            string variableName = Text[..dotIndex];
            Variable variable = Memory.Instance.GetVariable(variableName);
            if (variable == null)
                return null;
            if (!variable.Initialised)
                return null;
            if (!variable._VarType.IsArray)
                return null;
            string stringAfterDot = Text[(dotIndex + 1)..];
            if (stringAfterDot != "Length")
                return null;
            ArrayLength arrayLength = new(variable);
            arrayLengthIsNull = false;
            return arrayLength;
        }

        public Operator _Operator { get; init; }

        // null if not applicable
        public KeyString[] SubKeyStrings { get; init; }

        private static string[] nonKeywordKeyStrings = new string[]
        {
            "=",
            "{",
            "}",
            ",",
            "+",
            "-",
            "*",
            "/",
            "<",
            ">",
            "(",
            ")"
        };

        public static string[] GetKeyStringsAsStrings(string text)
        {
            // ensure that every key string is surrounded by at least one space on either side
            foreach (string keyString in nonKeywordKeyStrings)
                text = text.Replace(keyString, $" {keyString} ");

            text = RemoveWhiteSpaceSurroundingCharacter(text, '[', Direction.LeftRight);
            text = RemoveWhiteSpaceSurroundingCharacter(text, ']', Direction.Left);
            text = RemoveWhiteSpaceSurroundingCharacter(text, '.', Direction.LeftRight);
            text = RemoveWhiteSpaceBetweenCharacters(text, '[', ']');

            // remove all whitespace characters and return everything else, separated
            return
                text
                .Split(default(char[]), StringSplitOptions.TrimEntries)
                .Where(str => !string.IsNullOrWhiteSpace(str))
                .ToArray();
        }

        private static string RemoveWhiteSpaceSurroundingCharacter(string input, char character, Direction direction)
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

        private static string RemoveWhiteSpaceBetweenCharacters(string input, char char1, char char2)
        {
            int i = 0;
            int bracketsOpened = 0;
            int lastIndexOfChar2 = input.LastIndexOf(char2);
            while (i < input.Length)
            {
                if (input[i] == char1)
                {
                    bracketsOpened++;
                    i++;
                    continue;
                }
                if (input[i] == char2)
                {
                    bracketsOpened--;
                    if (bracketsOpened < 0)
                        bracketsOpened = 0;
                    i++;
                    continue;
                }
                if (bracketsOpened == 0)
                {
                    i++;
                    continue;
                }
                if (!char.IsWhiteSpace(input[i]))
                {
                    i++;
                    continue;
                }
                if (i >= lastIndexOfChar2)
                {
                    i++;
                    continue;
                }
                input = input.Remove(i, 1);
            }
            return input;
        }

        private enum Direction
        {
            Left,
            Right,
            LeftRight
        }

        private static string RemoveWhiteSpaceSurroundingIndex(string input, int index, Direction direction, out int newIndex)
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
                    throw new KeyStringException(null,
                        $"Internal exception: Invalid direction {direction}");
            }
        }

        private static string RemoveWhiteSpaceLeftOfIndex(string input, int index, out int newIndex)
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

        private static string RemoveWhiteSpaceRightOfIndex(string input, int index)
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
            if (IsType)                 return Type.Type;
            if (IsVariable)             return Type.Variable;
            if (IsEquals)               return Type.Equals;
            if (IsInteger)              return Type.Integer;
            if (IsBoolean)              return Type.Boolean;
            if (IsCharacter)            return Type.Character;
            if (IsDoubleFloat)          return Type.DoubleFloat;
            if (IsNewKeyword)           return Type.NewKeyword;
            if (IsArrayConstruction)    return Type.ArrayConstruction;
            if (IsOpenSquareBrace)      return Type.OpenSquareBrace;
            if (IsCloseSquareBrace)     return Type.CloseSquareBrace;
            if (IsOpenCurlyBrace)       return Type.OpenCurlyBrace;
            if (IsCloseCurlyBrace)      return Type.CloseCurlyBrace;
            if (IsComma)                return Type.Comma;
            if (IsArrayElement)         return Type.ArrayElement;
            if (IsNull)                 return Type.Null;
            if (IsArrayLength)          return Type.ArrayLength;
            if (IsDot)                  return Type.Dot;
            if (IsLengthProperty)       return Type.LengthProperty;
            if (IsOperator)             return Type.Operator;
            if (IsOpenBracket)          return Type.OpenBracket;
            if (IsCloseBracket)         return Type.CloseBracket;
                                        return Type.Invalid;
        }

        private bool IsKeyword { get => IsType || IsBoolean || IsNewKeyword || IsNull; }

        private bool IsType { get => _isType ??= VarType.GetVarType(Text) != null; }
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

        private bool IsNewKeyword { get => _isNewKeyword ??= Text == "new"; }
        private bool? _isNewKeyword;

        private bool IsArrayConstruction { get => _isArrayConstruction ??= _ArrayConstruction != null; }
        private bool? _isArrayConstruction;

        private bool IsOpenSquareBrace { get => _isOpenSquareBrace ??= Text == "["; }
        private bool? _isOpenSquareBrace;

        private bool IsCloseSquareBrace { get => _isCloseSquareBrace ??= Text == "]"; }
        private bool? _isCloseSquareBrace;

        private bool IsOpenCurlyBrace { get => _isOpenCurlyBrace ??= Text == "{"; }
        private bool? _isOpenCurlyBrace;

        private bool IsCloseCurlyBrace { get => _isCloseCurlyBrace ??= Text == "}"; }
        private bool? _isCloseCurlyBrace;

        private bool IsComma { get => _isComma ??= Text == ","; }
        private bool? _isComma;

        private bool IsArrayElement { get => _isArrayElement ??= _ArrayElement != null; }
        private bool? _isArrayElement;

        private bool IsNull { get => _isNull ??= Text == "null"; }
        private bool? _isNull;

        private bool IsArrayLength { get => _isArrayLength ??= _ArrayLength != null; }
        private bool? _isArrayLength;

        private bool IsDot { get => _isDot ??= Text == "."; }
        private bool? _isDot;

        private bool IsLengthProperty { get => _isLengthProperty ??= Text == "Length"; }
        private bool? _isLengthProperty;

        private bool IsOperator { get => _isOperator ??= Operator.GetOperator(Text) != null; }
        private bool? _isOperator;

        private bool IsOpenBracket { get => _isOpenBracket ??= Text == "("; }
        private bool? _isOpenBracket;

        private bool IsCloseBracket { get => _isCloseBracket ??= Text == ")"; }
        private bool? _isCloseBracket;

        public enum Type
        {
            Invalid,
            Type,
            Variable,
            Equals,
            Integer,
            Boolean,
            Character,
            DoubleFloat,
            NewKeyword,
            ArrayConstruction,
            OpenSquareBrace,
            CloseSquareBrace,
            OpenCurlyBrace,
            CloseCurlyBrace,
            Comma,
            ArrayElement,
            Null,
            ArrayLength,
            Dot,
            LengthProperty,
            Operator,
            OpenBracket,
            CloseBracket
        }

        public override string ToString() => Text;

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
