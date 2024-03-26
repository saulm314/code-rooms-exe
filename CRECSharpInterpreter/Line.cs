using System;

namespace CRECSharpInterpreter
{
    public class Line
    {
        public Line(string text)
        {
            Text = text;
            string[] keyStringsStr = KeyStringSeparator.GetKeyStringsAsStrings(Text);
            KeyStrings = new KeyString[keyStringsStr.Length];
            for (int i = 0; i < KeyStrings.Length; i++)
                KeyStrings[i] = new(keyStringsStr[i]);

            _Type = GetType();
            switch (_Type)
            {
                case Type.Invalid:
                    throw new LineException(this, $"Unrecognised operation in line:\n{Text}");
                case Type.WriteStringElement:
                case Type.WriteArrayStringElement:
                    throw new LineException(this, "Cannot write to an element of a string because strings are immutable");
                case Type.EmptyLine:
                    break;
                case Type.Declaration:
                    VerifyDeclarationValid();
                    DeclareVariable();
                    break;
                case Type.DeclarationInitialisation:
                    VerifyDeclarationValid();
                    _Expression = CreateExpression(); // must be done before declaring variable to ensure no self-references
                    VarToWrite = DeclareVariable();
                    VerifyWriteVariableValid();
                    VarToWrite.Initialised = true;
                    break;
                case Type.WriteVariable:
                    _Expression = CreateExpression();
                    VarToWrite = GetVarToWrite_NoDeclaration();
                    VerifyWriteVariableValid();
                    VarToWrite.Initialised = true;
                    break;
                case Type.WriteArrayElement:
                    _Expression = CreateExpression();
                    ElementToWrite = GetElementToWrite();
                    VerifyWriteArrayElementValid();
                    break;
                case Type.IfSingleLine:
                case Type.IfMultiLine:
                    SubLinesStr = GetSubLinesIfWhile(out Line header);
                    Header = header;
                    SubLines = new Line[SubLinesStr.Length];
                    if (Memory.Instance!._Mode == Mode.Compilation)
                    {
                        // these two lines cancel each other out
                        // however in running them we verify that there are no compilation errors
                        CreateSubLines();
                        Memory.Instance.PopFromStack();
                    }
                    break;
                case Type.IfElse:
                    SubLinesStr = GetSubLinesIfWhile(out Line _header);
                    Header = _header;
                    SubLines = new Line[SubLinesStr.Length];
                    if (Memory.Instance!._Mode == Mode.Compilation)
                    {
                        CreateSubLines();
                        Memory.Instance.PopFromStack();
                    }
                    SubLinesStr2 = GetSubLinesElse(out Line _header2);
                    Header2 = _header2;
                    SubLines2 = new Line[SubLinesStr2.Length];
                    if (Memory.Instance._Mode == Mode.Compilation)
                    {
                        CreateSubLines2();
                        Memory.Instance.PopFromStack();
                    }
                    break;
                case Type.If:
                    _Expression = CreateExpressionIfWhile();
                    Memory.Instance!.PushToStack();
                    Condition = DeclareConditionVariable();
                    break;
                case Type.Else:
                    // at runtime else will use the same scope as otherwise if would have used
                    // but at compile time we need to check the two scopes separately
                    // so we create a temporary scope
                    if (Memory.Instance!._Mode == Mode.Compilation)
                        Memory.Instance.PushToStack();
                    break;
            }
        }

        public string Text { get; init; }

        public KeyString[] KeyStrings { get; init; }

        public Type _Type { get; init; }

        public Variable? VarToWrite { get; init; }

        public ArrayElement? ElementToWrite { get; init; }

        public Expression? _Expression { get; init; }

        public Line? Header { get; init; }
        public Line? Header2 { get; init; }

        public string[]? SubLinesStr { get; init; }
        public Line[]? SubLines { get; private set; }
        public string[]? SubLinesStr2 { get; init; }
        public Line[]? SubLines2 { get; private set; }

        public Variable? Condition { get; init; }

        public bool Executed { get; private set; } = false;
        public void Execute()
        {
            string separator = Interpreter.SEPARATOR;
            switch (_Type)
            {
                case Type.IfSingleLine:
                case Type.IfMultiLine:
                case Type.IfElse:
                    break;
                default:
                    Console.WriteLine(Text.TrimStart() + '\n');
                    break;
            }
            switch (_Type)
            {
                case Type.DeclarationInitialisation:
                case Type.WriteVariable:
                    PerformWriteVariable();
                    break;
                case Type.WriteArrayElement:
                    PerformWriteArrayElement();
                    break;
                case Type.IfSingleLine:
                case Type.IfMultiLine:
                    ExecuteIf();
                    if (Executed)
                        Memory.Instance!.PopFromStack();
                    return;
                case Type.IfElse:
                    ExecuteIfElse();
                    if (Executed)
                        Memory.Instance!.PopFromStack();
                    return;
                case Type.If:
                    EvaluateIfWhile();
                    break;
                case Type.Else:
                    break;
            }
            Executed = true;
            Console.WriteLine("Stack:");
            Console.WriteLine(separator + "\n");
            Scope[] scopes = Memory.Instance!.Stack.ToArray();

            for (int i = scopes.Length - 1; i >= 0; i--)
            {
                Scope scope = scopes[i];
                foreach (Variable variable in scope.DeclaredVariables)
                    Console.WriteLine(variable);
                Console.WriteLine(separator + "\n");
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

        private void EvaluateIfWhile()
        {
            _Expression!.Compute();
            Condition!.Value = _Expression.Value;
        }

        private void ExecuteIf()
        {
            if (!Header!.Executed)
            {
                Header.Execute();
                if (!(bool)Header.Condition!.Value!)
                    Executed = true;
                if (SubLines!.Length == 0)
                    Executed = true;
                return;
            }
            ExecuteNextSubLine();
            if (subLinesExecuted == SubLines!.Length)
                Executed = true;
        }

        private void ExecuteIfElse()
        {
            if (!Header!.Executed)
            {
                Header.Execute();
                if ((bool)Header.Condition!.Value! && SubLines!.Length == 0)
                    Executed = true;
                return;
            }
            if ((bool)Header.Condition!.Value!)
            {
                ExecuteNextSubLine();
                if (subLinesExecuted == SubLines!.Length)
                    Executed = true;
                return;
            }
            if (!Header2!.Executed)
            {
                Header2.Execute();
                if (!(bool)Header.Condition!.Value! && SubLines2!.Length == 0)
                    Executed = true;
                return;
            }
            ExecuteNextSubLine2();
            if (subLinesExecuted2 == SubLines2!.Length)
                Executed = true;
        }

        private int subLinesExecuted = 0;
        private void ExecuteNextSubLine()
        {
            SubLines![subLinesExecuted] ??= new(SubLinesStr![subLinesExecuted]);
            SubLines![subLinesExecuted].Execute();
            if (SubLines[subLinesExecuted].Executed)
                subLinesExecuted++;
        }

        private int subLinesExecuted2 = 0;
        private void ExecuteNextSubLine2()
        {
            SubLines2![subLinesExecuted2] ??= new(SubLinesStr2![subLinesExecuted2]);
            SubLines2![subLinesExecuted2].Execute();
            if (SubLines2[subLinesExecuted2].Executed)
                subLinesExecuted2++;
        }

        private void VerifyDeclarationValid()
        {
            string varName = KeyStrings[1].Text;
            if (Memory.Instance!.IsDeclared(varName))
                throw new LineException(this, $"Variable {varName} has already been declared");
        }

        private Variable DeclareVariable()
        {
            VarType varType = VarType.GetVarType(KeyStrings[0].Text)!;
            string varName = KeyStrings[1].Text;
            Variable declaredVariable = new(varType, varName);
            Memory.Instance!.AddToCurrentScope(declaredVariable);
            return declaredVariable;
        }

        private Variable DeclareConditionVariable()
        {
            int stackSize = Memory.Instance!.Stack.Count;
            string conditionName = $"[Condition{stackSize - 2}]";
            Variable condition = new(VarType.@bool, conditionName);
            Memory.Instance!.AddToCurrentScope(condition);
            return condition;
        }

        private Expression CreateExpression()
        {
            int expressionOffset = _Type switch
            {
                Type.DeclarationInitialisation => 3,
                Type.WriteVariable => 2,
                Type.WriteArrayElement => 2,
                _ => throw new LineException(this, "internal error")
            };
            KeyString[] expressionKeyStrings = new KeyString[KeyStrings.Length - expressionOffset - 1];
            // not copying the final semicolon
            Array.Copy(KeyStrings, expressionOffset, expressionKeyStrings, 0, expressionKeyStrings.Length);
            return new(expressionKeyStrings);
        }

        private Expression CreateExpressionIfWhile()
        {
            KeyString[] keyStrings = new KeyString[KeyStrings.Length - 1];
            Array.Copy(KeyStrings, 1, keyStrings, 0, keyStrings.Length);
            Expression expression = new(keyStrings);
            if (expression._VarType != VarType.@bool)
                throw new LineException(this, "Expression on the header of an if/while statement must be a boolean!");
            return expression;
        }

        private string[] GetSubLinesIfWhile(out Line header)
        {
            string? headerStr;
            string[] subLinesStr = _Type switch
            {
                Type.IfSingleLine => LineSeparator.GetSubLinesAsStringsIfSingleLine(Text, out headerStr),
                Type.IfMultiLine => LineSeparator.GetSubLinesAsStringsIfMultiLine(Text, out headerStr),
                Type.IfElse => LineSeparator.GetSubLinesAsStringsIfElseIf(Text, out headerStr),
                _ => throw new LineException(this, "Internal error")
            };
            header = new(headerStr);
            return subLinesStr;
        }

        private string[] GetSubLinesElse(out Line header)
        {
            string[] subLinesStr = LineSeparator.GetSubLinesAsStringsIfElseElse(Text, out string? headerStr);
            header = new(headerStr);
            return subLinesStr;
        }

        private void CreateSubLines()
        {
            for (int i = 0; i < SubLines!.Length; i++)
                SubLines[i] = new(SubLinesStr![i]);
        }

        private void CreateSubLines2()
        {
            for (int i = 0; i < SubLines2!.Length; i++)
                SubLines2[i] = new(SubLinesStr2![i]);
        }

        private void VerifyWriteVariableValid()
        {
            if (_Expression!._VarType == null)
            {
                if (VarToWrite!._VarType!.DefaultValue == null)
                    return;
                throw new LineException(this,
                    $"Cannot write null to the value type variable \"{VarToWrite.Name}\"");
            }
            if (VarToWrite!._VarType != _Expression._VarType)
                throw new LineException(this,
                    $"Cannot write expression of type {_Expression._VarType} to variable {VarToWrite.Name} of type {VarToWrite._VarType}");
        }

        private void VerifyWriteArrayElementValid()
        {
            if (_Expression!._VarType == null)
            {
                if (ElementToWrite!.Array._VarType!.Unarray!.DefaultValue == null)
                    return;
                throw new LineException(this,
                    $"Cannot write null to a value type element of array \"{ElementToWrite.Array.Name}\"");
            }
            if (ElementToWrite!.Array._VarType!.Unarray != _Expression._VarType)
                throw new LineException(this,
                    $"Cannot write expression of type {_Expression._VarType} to element of array " +
                    $"{ElementToWrite.Array.Name} of type {ElementToWrite.Array._VarType.Unarray}");
        }

        private Variable GetVarToWrite_NoDeclaration()
        {
            Variable? varToWrite = Memory.Instance!.GetVariable(KeyStrings[0].Text);
            if (varToWrite == null)
                throw new LineException(this, $"Variable {KeyStrings[0].Text} hasn't been declared");
            return varToWrite;
        }

        private ArrayElement GetElementToWrite()
        {
            return KeyStrings[0]._ArrayElement!;
        }

        private void PerformWriteVariable()
        {
            bool isReferenceType = VarToWrite!._VarType!._Storage == VarType.Storage.Reference;
            bool isNull = VarToWrite.Value == null;
            if (isReferenceType && !isNull)
            {
                int oldAllocation = (int)VarToWrite.Value!;
                Memory.Instance!.Heap.DecrementReferenceCounter(oldAllocation);
            }

            _Expression!.Compute();

            VarToWrite.Value = _Expression.Value;
        }

        private void PerformWriteArrayElement()
        {
            if (ElementToWrite!.Array.Value == null)
                throw new LineException(this, $"Array \"{ElementToWrite.Array.Name}\" has value null");
            int heapIndex = (int)ElementToWrite.Array.Value;
            ElementToWrite.IndexExpression.Compute();
            ElementToWrite.Index = (int)ElementToWrite.IndexExpression.Value!;
            int index = ElementToWrite.Index;

            bool isReferenceType = ElementToWrite.Array._VarType!.Unarray!._Storage == VarType.Storage.Reference;
            bool isNull = Memory.Instance!.Heap.GetValue(heapIndex, index) == null;
            if (isReferenceType && !isNull)
            {
                int oldAllocation = (int)Memory.Instance!.Heap.GetValue(heapIndex, index)!;
                Memory.Instance.Heap.DecrementReferenceCounter(oldAllocation);
            }

            _Expression!.Compute();

            Memory.Instance.Heap.SetValue(heapIndex, index, _Expression.Value);
        }

        private new Type GetType()
        {
            if (IsEmptyLine)                    return Type.EmptyLine;
            if (IsDeclaration)                  return Type.Declaration;
            if (IsDeclarationInitialisation)    return Type.DeclarationInitialisation;
            if (IsWriteVariable)                return Type.WriteVariable;
            if (IsWriteArrayElement)            return Type.WriteArrayElement;
            if (IsWriteStringElement)           return Type.WriteStringElement;
            if (IsWriteArrayStringElement)      return Type.WriteArrayStringElement;
            if (IsIfSingleLine)                 return Type.IfSingleLine;
            if (IsIfMultiLine)                  return Type.IfMultiLine;
            if (IsIf)                           return Type.If;
            if (IsElse)                         return Type.Else;
            if (IsIfElse)                       return Type.IfElse;
                                                return Type.Invalid;
        }

        private bool IsEmptyLine
        {
            get =>
                _isEmptyLine ??=
                    KeyStrings.Length == 1 &&
                    KeyStrings[0]._Type == KeyString.Type.Semicolon;
        }
        private bool? _isEmptyLine;

        private bool IsDeclaration
        {
            get =>
                _isDeclaration ??=
                    KeyStrings.Length == 3 &&
                    KeyStrings[0]._Type == KeyString.Type.Type &&
                    KeyStrings[1]._Type == KeyString.Type.Variable &&
                    KeyStrings[2]._Type == KeyString.Type.Semicolon;
        }
        private bool? _isDeclaration;

        private bool IsDeclarationInitialisation
        {
            get =>
                _isDeclarationInitialisation ??=
                    KeyStrings.Length >= 5 &&
                    KeyStrings[0]._Type == KeyString.Type.Type &&
                    KeyStrings[1]._Type == KeyString.Type.Variable &&
                    KeyStrings[2]._Type == KeyString.Type.Equals &&
                    KeyStrings[KeyStrings.Length - 1]._Type == KeyString.Type.Semicolon;
        }
        private bool? _isDeclarationInitialisation;

        private bool IsWriteVariable
        {
            get =>
                _isWriteVariable ??=
                    KeyStrings.Length >= 4 &&
                    KeyStrings[0]._Type == KeyString.Type.Variable &&
                    KeyStrings[1]._Type == KeyString.Type.Equals &&
                    KeyStrings[KeyStrings.Length - 1]._Type == KeyString.Type.Semicolon;
        }
        private bool? _isWriteVariable;

        private bool IsWriteArrayElement
        {
            get =>
                _isWriteArrayElement ??=
                    KeyStrings.Length >= 4 &&
                    KeyStrings[0]._Type == KeyString.Type.ArrayElement &&
                    KeyStrings[1]._Type == KeyString.Type.Equals &&
                    KeyStrings[KeyStrings.Length - 1]._Type == KeyString.Type.Semicolon;
        }
        private bool? _isWriteArrayElement;

        private bool IsWriteStringElement
        {
            get =>
                _isWriteStringElement ??=
                    KeyStrings.Length >= 4 &&
                    KeyStrings[0]._Type == KeyString.Type.StringElement &&
                    KeyStrings[1]._Type == KeyString.Type.Equals &&
                    KeyStrings[KeyStrings.Length - 1]._Type == KeyString.Type.Semicolon;
        }
        private bool? _isWriteStringElement;

        private bool IsWriteArrayStringElement
        {
            get =>
                _isWriteArrayStringElement ??=
                    KeyStrings.Length >= 4 &&
                    KeyStrings[0]._Type == KeyString.Type.ArrayStringElement &&
                    KeyStrings[1]._Type == KeyString.Type.Equals &&
                    KeyStrings[KeyStrings.Length - 1]._Type == KeyString.Type.Semicolon;
        }
        private bool? _isWriteArrayStringElement;

        private bool IsIf
        {
            get =>
                _isIf ??=
                    KeyStrings.Length >= 4 &&
                    KeyStrings[0]._Type == KeyString.Type.IfKeyword &&
                    KeyStrings[1]._Type == KeyString.Type.OpenBracket &&
                    KeyStrings[KeyStrings.Length - 1]._Type == KeyString.Type.CloseBracket;
        }
        private bool? _isIf;

        private bool IsElse
        {
            get =>
                _isElse ??=
                    KeyStrings.Length == 1 &&
                    KeyStrings[0]._Type == KeyString.Type.ElseKeyword;
        }
        private bool? _isElse;

        private bool IsIfSingleLine
        {
            get =>
                _isIfSingleLine ??=
                    KeyStrings.Length >= 5 &&
                    KeyStrings[0]._Type == KeyString.Type.IfKeyword &&
                    KeyStrings[1]._Type == KeyString.Type.OpenBracket &&
                    Array.Exists(KeyStrings, keyString => keyString._Type == KeyString.Type.CloseBracket) &&
                    KeyStrings[KeyStrings.Length - 1]._Type == KeyString.Type.Semicolon &&
                    !Array.Exists(KeyStrings, keyString => keyString._Type == KeyString.Type.ElseKeyword);
        }
        private bool? _isIfSingleLine;

        private bool IsIfMultiLine
        {
            get =>
                _isIfMultiLine ??=
                    KeyStrings.Length >= 6 &&
                    KeyStrings[0]._Type == KeyString.Type.IfKeyword &&
                    KeyStrings[1]._Type == KeyString.Type.OpenBracket &&
                    Array.Exists(KeyStrings, keyString => keyString._Type == KeyString.Type.CloseBracket) &&
                    Array.Exists(KeyStrings, keyString => keyString._Type == KeyString.Type.OpenCurlyBrace) &&
                    KeyStrings[KeyStrings.Length - 1]._Type == KeyString.Type.CloseCurlyBrace &&
                    !Array.Exists(KeyStrings, keyString => keyString._Type == KeyString.Type.ElseKeyword);
        }
        private bool? _isIfMultiLine;

        private bool IsIfElse
        {
            get =>
                _isIfElse ??=
                    KeyStrings.Length >= 7 &&
                    KeyStrings[0]._Type == KeyString.Type.IfKeyword &&
                    KeyStrings[1]._Type == KeyString.Type.OpenBracket &&
                    Array.Exists(KeyStrings, keyString => keyString._Type == KeyString.Type.CloseBracket) &&
                    Array.Exists(KeyStrings, keyString => keyString._Type == KeyString.Type.ElseKeyword);
        }
        private bool? _isIfElse;

        public enum Type
        {
            Invalid,
            EmptyLine,
            Declaration,
            DeclarationInitialisation,
            WriteVariable,
            WriteArrayElement,
            WriteStringElement,
            WriteArrayStringElement,
            If,
            Else,
            IfSingleLine,
            IfMultiLine,
            IfElse
        }

        public class LineException : InterpreterException
        {
            public LineException(Line? line, string? message = null) : base(message)
            {
                this.line = line;
            }

            public Line? line;
        }
    }
}
