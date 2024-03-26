using System;
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
                    SubKeyStrings = new KeyString[3 + _ArrayConstruction!.ArrayLengthExpression.KeyStrings.Length];
                    SubKeyStrings[0] = new(_ArrayConstruction._VarType.Unarray!.Name);
                    SubKeyStrings[1] = new("[");
                    _ArrayConstruction.ArrayLengthExpression.KeyStrings.CopyTo(SubKeyStrings, 2);
                    SubKeyStrings[SubKeyStrings.Length - 1] = new("]");
                    break;
                case Type.ArrayElement:
                    SubKeyStrings = new KeyString[3 + _ArrayElement!.IndexExpression.KeyStrings.Length];
                    SubKeyStrings[0] = new(_ArrayElement.Array.Name!);
                    SubKeyStrings[1] = new("[");
                    _ArrayElement.IndexExpression.KeyStrings.CopyTo(SubKeyStrings, 2);
                    SubKeyStrings[SubKeyStrings.Length - 1] = new("]");
                    break;
                case Type.StringElement:
                    SubKeyStrings = new KeyString[3 + _StringElement!.IndexExpression.KeyStrings.Length];
                    SubKeyStrings[0] = new(_StringElement.String.Name!);
                    SubKeyStrings[1] = new("[");
                    _StringElement.IndexExpression.KeyStrings.CopyTo(SubKeyStrings, 2);
                    SubKeyStrings[SubKeyStrings.Length - 1] = new("]");
                    break;
                case Type.ArrayLength:
                    SubKeyStrings = new KeyString[3];
                    SubKeyStrings[0] = new(_ArrayLength!.Array.Name!);
                    SubKeyStrings[1] = new(".");
                    SubKeyStrings[2] = new("Length");
                    break;
                case Type.StringLength:
                    SubKeyStrings = new KeyString[3];
                    SubKeyStrings[0] = new(_StringLength!.String.Name!);
                    SubKeyStrings[1] = new(".");
                    SubKeyStrings[2] = new("Length");
                    break;
                case Type.Operator:
                    _Operator = Operator.GetOperator(Text);
                    if (!_Operator!.IsCast)
                        break;
                    SubKeyStrings = new KeyString[3];
                    SubKeyStrings[0] = new("(");
                    SubKeyStrings[2] = new(")");
                    string varTypeAsString = Text[1..(Text.Length - 1)];
                    SubKeyStrings[1] = new(varTypeAsString);
                    break;
                case Type.StringLiteral:
                    _StringLiteral = new(Text);
                    break;
            }
        }

        public string Text { get; init; }

        public Type _Type { get; init; }

        // null if not a literal
        public Literal? _Literal { get; init; }

        public StringLiteral? _StringLiteral { get; init; }

        // null if not an array construction
        public ArrayConstruction? _ArrayConstruction { get => _arrayConstruction ??= CreateArrayConstruction(); }
        private ArrayConstruction? _arrayConstruction;
        private bool arrayConstructionIsNull;

        private ArrayConstruction? CreateArrayConstruction()
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
            VarType? varType = VarType.GetVarType(varTypeAsString);
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

        public StringElement? _StringElement { get => _stringElement ??= CreateStringElement(); }
        private StringElement? _stringElement;
        private bool stringElementIsNull;

        private StringElement? CreateStringElement()
        {
            if (stringElementIsNull)
                return null;
            stringElementIsNull = true;
            int openSquareBraceIndex = Text.IndexOf('[');
            if (openSquareBraceIndex == -1)
                return null;
            int closeSquareBraceIndex = Text.IndexOf(']');
            if (closeSquareBraceIndex == -1)
                return null;
            if (closeSquareBraceIndex <= openSquareBraceIndex)
                return null;
            string variableName = Text[..openSquareBraceIndex];
            Variable? variable = Memory.Instance!.GetVariable(variableName);
            if (variable == null)
                return null;
            if (!variable.Initialised)
                return null;
            if (variable._VarType != VarType.@string)
                return null;
            string stringInsideBraces = Text[(openSquareBraceIndex + 1)..closeSquareBraceIndex];
            if (string.IsNullOrWhiteSpace(stringInsideBraces))
                return null;
            StringElement stringElement = new(variable, stringInsideBraces);
            stringElementIsNull = false;
            return stringElement;
        }

        public ArrayStringElement? _ArrayStringElement { get => _arrayStringElement ??= CreateArrayStringElement(); }
        private ArrayStringElement? _arrayStringElement;
        private bool arrayStringElementIsNull;

        private ArrayStringElement? CreateArrayStringElement()
        {
            if (arrayStringElementIsNull)
                return null;
            arrayStringElementIsNull = true;
            int openSquareBraceIndex1 = Text.IndexOf('[');
            int openSquareBraceIndex2 = Text.IndexOf('[', openSquareBraceIndex1 + 1);
            int closeSquareBraceIndex1 = Text.IndexOf(']');
            int closeSquareBraceIndex2 = Text.IndexOf(']', closeSquareBraceIndex1 + 1);
            bool anySquareBraceDoesntExist =
                openSquareBraceIndex1 == -1 ||
                openSquareBraceIndex2 == -1 ||
                closeSquareBraceIndex1 == -1 ||
                closeSquareBraceIndex2 == -1;
            if (anySquareBraceDoesntExist)
                return null;
            if (openSquareBraceIndex2 != closeSquareBraceIndex1 + 1)
                return null;
            if (closeSquareBraceIndex1 <= openSquareBraceIndex1)
                return null;
            if (closeSquareBraceIndex2 <= openSquareBraceIndex2)
                return null;
            string variableName = Text[..openSquareBraceIndex1];
            Variable? variable = Memory.Instance!.GetVariable(variableName);
            if (variable == null)
                return null;
            if (!variable.Initialised)
                return null;
            if (variable._VarType != VarType.@string.Array)
                return null;
            string stringInsideBraces1 = Text[(openSquareBraceIndex1 + 1)..closeSquareBraceIndex1];
            string stringInsideBraces2 = Text[(openSquareBraceIndex2 + 1)..closeSquareBraceIndex2];
            if (string.IsNullOrWhiteSpace(stringInsideBraces1))
                return null;
            if (string.IsNullOrWhiteSpace(stringInsideBraces2))
                return null;
            string stringAfterBraces = Text[(closeSquareBraceIndex2 + 1)..];
            if (!string.IsNullOrWhiteSpace(stringAfterBraces))
                return null;
            ArrayStringElement arrayStringElement = new(variable, stringInsideBraces1, stringInsideBraces2);
            arrayStringElementIsNull = false;
            return arrayStringElement;
        }

        public ArrayElement? _ArrayElement { get => _arrayElement ??= CreateArrayElement(); }
        private ArrayElement? _arrayElement;
        private bool arrayElementIsNull;

        private ArrayElement? CreateArrayElement()
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
            Variable? variable = Memory.Instance!.GetVariable(variableName);
            if (variable == null)
                return null;
            if (!variable.Initialised)
                return null;
            if (!variable._VarType!.IsArray)
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

        public StringLength? _StringLength { get => _stringLength ??= CreateStringLength(); }
        private StringLength? _stringLength;
        private bool stringLengthIsNull;

        private StringLength? CreateStringLength()
        {
            if (stringLengthIsNull)
                return null;
            stringLengthIsNull = true;
            int dotIndex = Text.IndexOf('.');
            if (dotIndex == -1)
                return null;
            string variableName = Text[..dotIndex];
            Variable? variable = Memory.Instance!.GetVariable(variableName);
            if (variable == null)
                return null;
            if (!variable.Initialised)
                return null;
            if (variable._VarType != VarType.@string)
                return null;
            string stringAfterDot = Text[(dotIndex + 1)..];
            if (stringAfterDot != "Length")
                return null;
            StringLength stringLength = new(variable);
            stringLengthIsNull = false;
            return stringLength;
        }

        public ArrayLength? _ArrayLength { get => _arrayLength ??= CreateArrayLength(); }
        private ArrayLength? _arrayLength;
        private bool arrayLengthIsNull;

        private ArrayLength? CreateArrayLength()
        {
            if (arrayLengthIsNull)
                return null;
            arrayLengthIsNull = true;
            int dotIndex = Text.IndexOf('.');
            if (dotIndex == -1)
                return null;
            string variableName = Text[..dotIndex];
            Variable? variable = Memory.Instance!.GetVariable(variableName);
            if (variable == null)
                return null;
            if (!variable.Initialised)
                return null;
            if (!variable._VarType!.IsArray)
                return null;
            string stringAfterDot = Text[(dotIndex + 1)..];
            if (stringAfterDot != "Length")
                return null;
            ArrayLength arrayLength = new(variable);
            arrayLengthIsNull = false;
            return arrayLength;
        }

        public Operator? _Operator { get; init; }

        // null if not applicable
        public KeyString[]? SubKeyStrings { get; init; }

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
            if (IsStringLiteral)        return Type.StringLiteral;
            if (IsStringElement)        return Type.StringElement;
            if (IsArrayStringElement)   return Type.ArrayStringElement;
            if (IsSemicolon)            return Type.Semicolon;
            if (IsStringLength)         return Type.StringLength;
            if (IsIfKeyword)            return Type.IfKeyword;
            if (IsElseKeyword)          return Type.ElseKeyword;
            if (IsWhileKeyword)         return Type.WhileKeyword;
                                        return Type.Invalid;
        }

        private bool IsKeyword
        {
            get =>
                IsType ||
                IsBoolean ||
                IsNewKeyword ||
                IsNull ||
                IsIfKeyword ||
                IsElseKeyword ||
                IsWhileKeyword;
        }

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

        private bool IsStringElement { get => _isStringElement ??= _StringElement != null; }
        private bool? _isStringElement;

        private bool IsArrayStringElement { get => _isArrayStringElement ??= _ArrayStringElement != null; }
        private bool? _isArrayStringElement;

        private bool IsNull { get => _isNull ??= Text == "null"; }
        private bool? _isNull;

        private bool IsArrayLength { get => _isArrayLength ??= _ArrayLength != null; }
        private bool? _isArrayLength;

        private bool IsStringLength { get => _isStringLength ??= _StringLength != null; }
        private bool? _isStringLength;

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

        private bool IsStringLiteral
        {
            get
            {
                if (_isStringLiteral is not null)
                    return (bool)_isStringLiteral;
                if (Text[0] != '"')
                    return _isStringLiteral ??= false;
                if (Text[Text.Length - 1] != '"')
                    return _isStringLiteral ??= false;
                return _isStringLiteral ??= true;
            }
        }
        private bool? _isStringLiteral;

        private bool IsSemicolon { get => _isSemicolon ??= Text == ";"; }
        private bool? _isSemicolon;

        private bool IsIfKeyword { get => _isIfKeyword ??= Text == "if"; }
        private bool? _isIfKeyword;

        private bool IsElseKeyword { get => _isElseKeyword ??= Text == "else"; }
        private bool? _isElseKeyword;

        private bool IsWhileKeyword { get => _isWhileKeyword ??= Text == "while"; }
        private bool? _isWhileKeyword;

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
            CloseBracket,
            StringLiteral,
            StringElement,
            ArrayStringElement,
            Semicolon,
            StringLength,
            IfKeyword,
            ElseKeyword,
            WhileKeyword
        }

        public override string ToString() => Text;

        public class KeyStringException : InterpreterException
        {
            public KeyStringException(KeyString? keyString, string? message = null) : base(message)
            {
                this.keyString = keyString;
            }

            KeyString? keyString;
        }
    }
}
