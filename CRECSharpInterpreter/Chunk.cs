using System;
using System.Collections.Generic;
using System.Linq;

namespace CRECSharpInterpreter
{
    public class Chunk
    {
        public Chunk(string text)
        {
            Text = text;
            string trimmedText = Text.TrimEnd();
            if (trimmedText[trimmedText.Length - 1] != ';')
                throw new ChunkException(this, "Semicolon on final line expected");

            string[] linesStr = GetLines();
            Lines = new Line[linesStr.Length];
            for (int i = 0; i < Lines.Length; i++)
                Lines[i] = new(linesStr[i]);

            CompileLines();
        }

        public string Text { get; init; }

        public Line[] Lines { get; init; }

        private string[] GetLines()
            =>
                Text
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Where(str => !string.IsNullOrWhiteSpace(str))
                .ToArray();

        private void CompileLines()
        {
            foreach (Line line in Lines)
            {
                Variable varToWrite;
                switch (line._Type)
                {
                    case Line.Type.Declaration:
                        Info.DeclaredVariables.Add(line.DeclaredVariable);
                        break;
                    case Line.Type.DeclarationInitialisation:
                        Info.DeclaredVariables.Add(line.DeclaredVariable);
                        varToWrite = line.DeclaredVariable;
                        CompileAnyWrite(line, 3, varToWrite, varToWrite.Name);
                        varToWrite.Initialised = true;
                        break;
                    case Line.Type.Write:
                        varToWrite = Info.DeclaredVariables.Find(var => var.Name == line.KeyStrings[0].Text);
                        CompileAnyWrite(line, 2, varToWrite, line.KeyStrings[0].Text);
                        break;
                }
            }
        }

        // any write refers to the fact that both DeclarationInitialisation and Write lines
        //      write to a variable
        private void CompileAnyWrite(Line line, int expressionOffset, Variable varToWrite, string varToWriteName)
        {
            KeyString[] expressionKeyStrings = new KeyString[line.KeyStrings.Length - expressionOffset];
            Array.Copy(line.KeyStrings, expressionOffset, expressionKeyStrings, 0, expressionKeyStrings.Length);
            line._Expression = new(expressionKeyStrings);

            string errorMessage;
            bool isWriteValid = IsWriteValid(varToWrite, varToWriteName, line._Expression, out errorMessage);
            if (!isWriteValid)
                throw new Line.LineException(line, errorMessage);
        }

        private bool IsWriteValid(Variable varToWrite, string varToWriteName, Expression expression, out string errorMessage)
        {
            if (varToWrite == null)
            {
                errorMessage = $"Cannot find variable {varToWriteName} to write";
                return false;
            }
            if (varToWrite._VarType != expression._VarType)
            {
                errorMessage = $"Cannot write expression of type {expression._VarType} to variable of type {varToWrite._VarType}";
                return false;
            }
            errorMessage = null;
            return true;
        }

        public class ChunkException : Exception
        {
            public ChunkException(Chunk chunk, string message = null) : base(message)
            {
                this.chunk = chunk;
            }

            public Chunk chunk;
        }
    }
}
