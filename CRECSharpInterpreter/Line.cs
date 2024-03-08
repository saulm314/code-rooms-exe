﻿using System;
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
        }

        public string Text { get; init; }

        public KeyString[] KeyStrings { get; init; }

        public Type _Type { get; init; }

        public Variable VarToWrite { get; init; }

        public Expression _Expression { get; init; }

        private bool IsDeclarationValid(out string errorMessage)
        {
            string varName = KeyStrings[1].Text;
            if (Info.DeclaredVariables.Exists(var => var.Name == varName))
            {
                errorMessage = $"Variable {varName} has already been declared";
                return false;
            }
            errorMessage = null;
            return true;
        }

        private void VerifyDeclarationValid()
        {
            string varName = KeyStrings[1].Text;
            if (Info.DeclaredVariables.Exists(var => var.Name == varName))
                throw new LineException(this, $"Variable {varName} has already been declared");
        }

        private Variable DeclareVariable()
        {
            VarType varType = VarType.VarTypes.Find(vt => vt.Name == KeyStrings[0].Text);
            string varName = KeyStrings[1].Text;
            Variable declaredVariable = new(varType, varName);
            Info.DeclaredVariables.Add(declaredVariable);
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
                    Variable variable = Info.DeclaredVariables.Find(var => var.Name == keyString.Text);
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
                throw new LineException(this, $"Cannot write expression of type {_Expression._VarType} to variable of type {VarToWrite._VarType}");
        }

        private Variable GetVarToWrite_NoDeclaration()
        {
            Variable varToWrite = Info.DeclaredVariables.Find(var => var.Name == KeyStrings[0].Text);
            if (varToWrite == null)
                throw new LineException(this, $"Variable {KeyStrings[0].Text} hasn't been declared");
            return varToWrite;
        }

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
