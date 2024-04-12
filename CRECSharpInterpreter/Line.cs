using System;

namespace CRECSharpInterpreter
{
    public class Line
    {
        public Line(string text, LineNumberInfo lineNumberInfo)
        {
            Text = text;
            ReducedText = LineNumberUtils.Trim(LineNumberUtils.RemoveSeparators(Text));
            _LineNumberInfo = lineNumberInfo;

            try
            {
                KeyStrings = GenerateKeyStrings();

                _Type = GetType();
                VerifyKeyStrings();
                switch (_Type)
                {
                    case Type.Invalid:
                        throw LineException.New(this, $"Unrecognised operation in line:\n{ReducedText}");
                    case Type.WriteStringElement:
                    case Type.WriteArrayStringElement:
                        throw LineException.New(this, "Cannot write to an element of a string because strings are immutable");
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
                        SubLinesStr = GetSubLinesIfWhileFor(out Line header, out LineNumberInfo[] subLineNumberInfos);
                        Header = header;
                        Header.Parent = this;
                        SubLines = new Line[SubLinesStr.Length];
                        SubLineNumberInfos = subLineNumberInfos;
                        if (Memory.Instance!._Mode == Mode.Compilation)
                        {
                            // these two lines cancel each other out
                            // however in running them we verify that there are no compilation errors
                            CreateSubLines();
                            Memory.Instance.PopFromStack();
                        }
                        break;
                    case Type.WhileSingleLine:
                    case Type.WhileMultiLine:
                        SubLinesStr = GetSubLinesIfWhileFor(out Line headerWhile, out LineNumberInfo[] subLineNumberInfosWhile);
                        Header = headerWhile;
                        Header.Parent = this;
                        SubLines = new Line[SubLinesStr.Length];
                        SubLineNumberInfos = subLineNumberInfosWhile;
                        if (Memory.Instance!._Mode == Mode.Compilation)
                        {
                            CreateSubLines();
                            Memory.Instance.PopLoopFromStack();
                        }
                        break;
                    case Type.ForSingleLine:
                    case Type.ForMultiLine:
                        SubLinesStr = GetSubLinesIfWhileFor(out Line headerFor, out LineNumberInfo[] subLineNumberInfosFor);
                        Header = headerFor;
                        Header.Parent = this;
                        SubLines = new Line[SubLinesStr.Length];
                        SubLineNumberInfos = subLineNumberInfosFor;
                        if (Memory.Instance!._Mode == Mode.Compilation)
                        {
                            CreateSubLines();
                            Memory.Instance.PopLoopFromStack();
                            Memory.Instance.PopFromStack();
                        }
                        break;
                    case Type.IfElse:
                        SubLinesStr = GetSubLinesIfWhileFor(out Line _header, out LineNumberInfo[] _subLineNumberInfos);
                        Header = _header;
                        Header.Parent = this;
                        SubLines = new Line[SubLinesStr.Length];
                        SubLineNumberInfos = _subLineNumberInfos;
                        if (Memory.Instance!._Mode == Mode.Compilation)
                        {
                            CreateSubLines();
                            Memory.Instance.PopFromStack();
                        }
                        SubLinesStr2 = GetSubLinesElse(out Line _header2, out LineNumberInfo[] _subLineNumberInfos2);
                        Header2 = _header2;
                        Header2.Parent = this;
                        SubLines2 = new Line[SubLinesStr2.Length];
                        SubLineNumberInfos2 = _subLineNumberInfos2;
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
                    case Type.While:
                        _Expression = CreateExpressionIfWhile();
                        Memory.Instance!.PushLoopToStack();
                        Condition = DeclareConditionVariable();
                        break;
                    case Type.For:
                        Memory.Instance!.PushToStack();
                        Initialiser = CreateInitialiser();
                        Initialiser.Parent = this;
                        Memory.Instance.PushLoopToStack();
                        _Expression = CreateExpressionFor();
                        Iterator = CreateIterator();
                        Iterator.Parent = this;
                        Condition = DeclareConditionVariable();
                        break;
                    case Type.Else:
                        // at runtime else will use the same scope as otherwise if would have used
                        // but at compile time we need to check the two scopes separately
                        // so we create a temporary scope
                        if (Memory.Instance!._Mode == Mode.Compilation)
                            Memory.Instance.PushToStack();
                        break;
                    case Type.Break:
                        Memory.Instance!.VerifyPopLoopValid();
                        break;
                    case Type.Continue:
                        Memory.Instance!.VerifyPopLoopValid();
                        break;
                }
            }
            catch (Exception e) when (e is not LineException)
            {
                //Console.WriteLine(e);
                throw LineException.New(this, null, e);
            }
        }

        private void VerifyKeyStrings()
        {
            // if it has sublines then let the sublines determine the exact keystrings
            if (HasSubLines)
                return;
            foreach (KeyString keyString in KeyStrings)
                if (keyString._Type == KeyString.Type.Invalid)
                    throw new KeyString.KeyStringException(keyString, $"Unrecognised key string: {keyString.Text}");
        }

        private bool HasSubLines
        {
            get =>
                _hasSubLines ??= _Type switch
                    {
                        Type.IfSingleLine => true,
                        Type.IfMultiLine => true,
                        Type.WhileSingleLine => true,
                        Type.WhileMultiLine => true,
                        Type.IfElse => true,
                        Type.ForSingleLine => true,
                        Type.ForMultiLine => true,
                        Type.For => true,
                        _ => false
                    };
        }
        private bool? _hasSubLines;

        private KeyString[] GenerateKeyStrings()
        {
            string[] keyStringsStr = KeyStringSeparator.GetKeyStringsAsStrings(ReducedText);
            KeyString[] keyStrings = new KeyString[keyStringsStr.Length];
            for (int i = 0; i < keyStrings.Length; i++)
                keyStrings[i] = new(keyStringsStr[i]);
            return keyStrings;
        }

        public string Text { get; init; }
        public string ReducedText { get; init; }

        public LineNumberInfo _LineNumberInfo { get; init; }

        public KeyString[] KeyStrings { get; private set; }

        public Type _Type { get; init; }

        public Variable? VarToWrite { get; init; }

        public ArrayElement? ElementToWrite { get; init; }

        public Line? Initialiser { get; init; }
        public Expression? _Expression { get; init; }
        public Line? Iterator { get; init; }

        public Line? Header { get; init; }
        public Line? Header2 { get; init; }

        public string[]? SubLinesStr { get; init; }
        public LineNumberInfo[]? SubLineNumberInfos { get; init; }
        public Line[]? SubLines { get; private set; }
        public string[]? SubLinesStr2 { get; init; }
        public LineNumberInfo[]? SubLineNumberInfos2 { get; init; }
        public Line[]? SubLines2 { get; private set; }

        public Variable? Condition { get; private set; }

        public Line? Parent { get; private set; }

        public bool Executed { get; private set; } = false;
        private bool continued = false;
        private bool broken = false;
        private bool ToReturn { get => Executed || continued || broken; }
        public void Execute()
        {
            try
            {
                _Execute();
            }
            catch (Exception e) when (e is not LineException)
            {
                throw LineException.New(this, null, e);
            }
        }
        private void _Execute()
        {
            string separator = Interpreter.SEPARATOR;
            switch (_Type)
            {
                case Type.IfSingleLine:
                case Type.IfMultiLine:
                case Type.IfElse:
                case Type.WhileSingleLine:
                case Type.WhileMultiLine:
                case Type.ForSingleLine:
                case Type.ForMultiLine:
                    break;
                default:
                    if (Parent?._Type == Type.For)
                        break;
                    Console.WriteLine(ReducedText + '\n');
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
                    if (continued || broken)
                        return;
                    if (Executed)
                        Memory.Instance!.PopFromStack();
                    return;
                case Type.WhileSingleLine:
                case Type.WhileMultiLine:
                    ExecuteWhile();
                    if (continued)
                    {
                        continued = false;
                        ContinueWhile();
                        return;
                    }
                    if (broken)
                        return;
                    if (Executed)
                        Memory.Instance!.PopLoopFromStack();
                    return;
                case Type.ForSingleLine:
                case Type.ForMultiLine:
                    ExecuteFor();
                    if (continued)
                    {
                        continued = false;
                        ContinueFor();
                        return;
                    }
                    if (broken)
                    {
                        return;
                    }
                    if (Executed)
                    {
                        Memory.Instance!.PopLoopFromStack();
                        Memory.Instance.PopFromStack();
                    }
                    return;
                case Type.IfElse:
                    ExecuteIfElse();
                    if (continued || broken)
                        return;
                    if (Executed)
                        Memory.Instance!.PopFromStack();
                    return;
                case Type.If:
                case Type.While:
                    EvaluateIfWhile();
                    break;
                case Type.For:
                    EvaluateFor();
                    break;
                case Type.Else:
                    break;
                case Type.Break:
                    ExecuteBreak();
                    break;
                case Type.Continue:
                    ExecuteContinue();
                    break;
            }
            Executed = true;
            if (Parent?._Type == Type.For)
                return;
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

        private void EvaluateFor()
        {
            if (!Initialiser!.Executed)
            {
                Initialiser.Execute();
                if (ToReturn)
                    return;
            }
            _Expression!.Compute();
            Condition!.Value = _Expression.Value;
        }

        private void ExecuteBreak()
        {
            Executed = true;
            broken = true;
            Line parent = Parent!;
            while
            (
                !(
                    parent._Type == Type.WhileSingleLine ||
                    parent._Type == Type.WhileMultiLine ||
                    parent._Type == Type.ForSingleLine ||
                    parent._Type == Type.ForMultiLine
                )
            )
            {
                parent.Executed = true;
                parent.broken = true;
                parent = parent.Parent!;
            }
            parent.Executed = true;
            parent.broken = true;
            Memory.Instance!.PopLoopFromStack();
            switch (parent._Type)
            {
                case Type.ForSingleLine:
                case Type.ForMultiLine:
                    Memory.Instance.PopFromStack();
                    break;
                default:
                    break;
            }
        }

        private void ExecuteContinue()
        {
            Executed = true;
            continued = true;
            Line parent = Parent!;
            while
            (
                !(
                    parent._Type == Type.WhileSingleLine ||
                    parent._Type == Type.WhileMultiLine ||
                    parent._Type == Type.ForSingleLine ||
                    parent._Type == Type.ForMultiLine
                )
            )
            {
                parent.Executed = true;
                parent.continued = true;
                parent = parent.Parent!;
            }
            parent.continued = true;
            Memory.Instance!.PopLoopFromStack();
        }

        private void ExecuteIf()
        {
            if (!Header!.Executed)
            {
                Header.Execute();
                // any time we call any execute method, we need to check
                // if the current line has executed or continued and return if so
                // this is to ensure that the code flows back to the last loop execution
                // in the event that a break or continue statement is used
                if (ToReturn)
                    return;
                if (!(bool)Header.Condition!.Value!)
                    Executed = true;
                if (SubLines!.Length == 0)
                    Executed = true;
                return;
            }
            ExecuteNextSubLine();
            if (ToReturn)
                return;
            if (subLinesExecuted == SubLines!.Length)
                Executed = true;
        }

        private void ExecuteWhile()
        {
            if (!Header!.Executed)
            {
                Header.Execute();
                if (ToReturn)
                    return;
                if (!(bool)Header.Condition!.Value!)
                    Executed = true;
                if (SubLines!.Length == 0)
                    Header.Executed = false;
                return;
            }
            ExecuteNextSubLine();
            if (ToReturn)
                return;
            if (subLinesExecuted == SubLines!.Length)
            {
                continued = true;
                Memory.Instance!.PopLoopFromStack();
            }
        }

        private void ExecuteFor()
        {
            if (!Header!.Executed)
            {
                Header.Execute();
                if (ToReturn)
                    return;
                if (!(bool)Header.Condition!.Value!)
                    Executed = true;
                if (SubLines!.Length == 0)
                {
                    Header.Iterator!.Execute();
                    if (ToReturn)
                        return;
                    Header.Executed = false;
                }
                return;
            }
            ExecuteNextSubLine();
            if (ToReturn)
                return;
            if (subLinesExecuted == SubLines!.Length)
            {
                continued = true;
                Memory.Instance!.PopLoopFromStack();
            }
        }

        private void ContinueWhile()
        {
            Header!.Executed = false;
            subLinesExecuted = 0;
            Array.Clear(SubLines!);
            Memory.Instance!.PushLoopToStack();
            Header.Condition = Header.DeclareConditionVariable();
        }

        private void ContinueFor()
        {
            Header!.Executed = false;
            subLinesExecuted = 0;
            Array.Clear(SubLines!);
            Memory.Instance!.PushLoopToStack();
            Header.Iterator!.Execute();
            if (ToReturn)
                return;
            Header.Condition = Header.DeclareConditionVariable();
        }

        private void ExecuteIfElse()
        {
            if (!Header!.Executed)
            {
                Header.Execute();
                if (ToReturn)
                    return;
                if ((bool)Header.Condition!.Value! && SubLines!.Length == 0)
                    Executed = true;
                return;
            }
            if ((bool)Header.Condition!.Value!)
            {
                ExecuteNextSubLine();
                if (ToReturn)
                    return;
                if (subLinesExecuted == SubLines!.Length)
                    Executed = true;
                return;
            }
            if (!Header2!.Executed)
            {
                Header2.Execute();
                if (ToReturn)
                    return;
                if (!(bool)Header.Condition!.Value! && SubLines2!.Length == 0)
                    Executed = true;
                return;
            }
            ExecuteNextSubLine2();
            if (ToReturn)
                return;
            if (subLinesExecuted2 == SubLines2!.Length)
                Executed = true;
        }

        private int subLinesExecuted = 0;
        private void ExecuteNextSubLine()
        {
            SubLines![subLinesExecuted] ??= new(SubLinesStr![subLinesExecuted], SubLineNumberInfos![subLinesExecuted]);
            SubLines[subLinesExecuted].Parent = this;
            SubLines[subLinesExecuted].Execute();
            if (ToReturn)
                return;
            if (SubLines[subLinesExecuted].Executed)
                subLinesExecuted++;
        }

        private int subLinesExecuted2 = 0;
        private void ExecuteNextSubLine2()
        {
            SubLines2![subLinesExecuted2] ??= new(SubLinesStr2![subLinesExecuted2], SubLineNumberInfos2![subLinesExecuted2]);
            SubLines2[subLinesExecuted2].Parent = this;
            SubLines2[subLinesExecuted2].Execute();
            if (ToReturn)
                return;
            if (SubLines2[subLinesExecuted2].Executed)
                subLinesExecuted2++;
        }

        private void VerifyDeclarationValid()
        {
            string varName = KeyStrings[1].Text;
            if (Memory.Instance!.IsDeclared(varName))
                throw LineException.New(this, $"Variable {varName} has already been declared");
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

        private Line CreateInitialiser()
        {
            string initialiserStr = LineSeparator.GetForInitialiserAsString(Text);
            Line initialiser = new(initialiserStr, _LineNumberInfo);
            switch (initialiser._Type)
            {
                case Type.DeclarationInitialisation:
                case Type.WriteVariable:
                case Type.WriteArrayElement:
                case Type.EmptyLine:
                    break;
                default:
                    throw LineException.New(this, "Invalid for loop initialiser");
            }
            return initialiser;
        }

        private Line CreateIterator()
        {
            string iteratorStr = LineSeparator.GetForIteratorAsString(Text);
            Line iterator = new(iteratorStr, _LineNumberInfo);
            switch (iterator._Type)
            {
                case Type.DeclarationInitialisation:
                case Type.WriteVariable:
                case Type.WriteArrayElement:
                case Type.EmptyLine:
                    break;
                default:
                    throw LineException.New(this, "Invalid for loop iterator");
            }
            return iterator;
        }

        private Expression CreateExpression()
        {
            int expressionOffset = _Type switch
            {
                Type.DeclarationInitialisation => 3,
                Type.WriteVariable => 2,
                Type.WriteArrayElement => 2,
                _ => throw LineException.New(this, "internal error")
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
                throw LineException.New(this, "Expression on the header of an if/while statement must be a boolean!");
            return expression;
        }

        private Expression CreateExpressionFor()
        {
            KeyStrings = GenerateKeyStrings();
            int firstSemicolonIndex = Array.FindIndex(KeyStrings, keyString => keyString._Type == KeyString.Type.Semicolon);
            int secondSemicolonIndex = Array.FindIndex(KeyStrings, firstSemicolonIndex + 1, keyString => keyString._Type == KeyString.Type.Semicolon);
            KeyString[] keyStrings = new KeyString[secondSemicolonIndex - firstSemicolonIndex - 1];
            Array.Copy(KeyStrings, firstSemicolonIndex + 1, keyStrings, 0, keyStrings.Length);
            Expression expression =
                keyStrings.Length != 0 ?
                new(keyStrings) :
                new(new KeyString[] { new KeyString("true") });
            if (expression._VarType != VarType.@bool)
                throw LineException.New(this, "For loop condition must be a boolean!");
            return expression;
        }

        private string[] GetSubLinesIfWhileFor(out Line header, out LineNumberInfo[] subLineNumberInfos)
        {
            string? headerStr;
            string[] subLinesStr = _Type switch
            {
                Type.IfSingleLine => LineSeparator.GetSubLinesAsStringsIfSingleLine(Text, out headerStr, _LineNumberInfo.LineNumber, out subLineNumberInfos),
                Type.IfMultiLine => LineSeparator.GetSubLinesAsStringsIfMultiLine(Text, out headerStr, _LineNumberInfo.LineNumber, out subLineNumberInfos),
                Type.IfElse => LineSeparator.GetSubLinesAsStringsIfElseIf(Text, out headerStr, _LineNumberInfo.LineNumber, out subLineNumberInfos),
                Type.WhileSingleLine => LineSeparator.GetSubLinesAsStringsIfSingleLine(Text, out headerStr, _LineNumberInfo.LineNumber, out subLineNumberInfos),
                Type.WhileMultiLine => LineSeparator.GetSubLinesAsStringsIfMultiLine(Text, out headerStr, _LineNumberInfo.LineNumber, out subLineNumberInfos),
                Type.ForSingleLine => LineSeparator.GetSubLinesAsStringsIfSingleLine(Text, out headerStr, _LineNumberInfo.LineNumber, out subLineNumberInfos),
                Type.ForMultiLine => LineSeparator.GetSubLinesAsStringsIfMultiLine(Text, out headerStr, _LineNumberInfo.LineNumber, out subLineNumberInfos),
                _ => throw LineException.New(this, "Internal error")
            };
            ushort[] headerLineNumbers = LineNumberUtils.GetLineNumbers(headerStr, out ushort actualLineNumber);
            LineNumberInfo headerLineNumberInfo;
            if (headerLineNumbers.Length > 0)
                headerLineNumberInfo = new(_LineNumberInfo.LineNumber, actualLineNumber, headerLineNumbers[^1]);
            else
                headerLineNumberInfo = new(_LineNumberInfo.LineNumber, _LineNumberInfo.ActualLineNumber, _LineNumberInfo.ActualLineNumber);
            header = new(headerStr, headerLineNumberInfo);
            return subLinesStr;
        }

        private string[] GetSubLinesElse(out Line header, out LineNumberInfo[] subLineNumberInfos)
        {
            string[] subLinesStr = LineSeparator.GetSubLinesAsStringsIfElseElse(Text, out string? headerStr, _LineNumberInfo.LineNumber, out subLineNumberInfos);
            ushort[] headerLineNumbers = LineNumberUtils.GetLineNumbers(headerStr, out ushort actualLineNumber);
            LineNumberInfo headerLineNumberInfo;
            if (headerLineNumbers.Length > 0)
                if (SubLineNumberInfos!.Length > 0)
                    headerLineNumberInfo = new(SubLineNumberInfos[^1].EndLineNumber, actualLineNumber, headerLineNumbers[^1]);
                else
                    headerLineNumberInfo = new(_LineNumberInfo.ActualLineNumber, actualLineNumber, headerLineNumbers[^1]);
            else
                if (SubLineNumberInfos!.Length > 0)
                {
                    ushort headerLineNumber = SubLineNumberInfos[^1].EndLineNumber;
                    headerLineNumberInfo = new(headerLineNumber, headerLineNumber, headerLineNumber);
                }
                else
                {
                    ushort headerLineNumber = Header!._LineNumberInfo.EndLineNumber;
                    headerLineNumberInfo = new(headerLineNumber, headerLineNumber, headerLineNumber);
                }
            header = new(headerStr, headerLineNumberInfo);
            return subLinesStr;
        }

        private void CreateSubLines()
        {
            for (int i = 0; i < SubLines!.Length; i++)
            {
                SubLines[i] = new(SubLinesStr![i], SubLineNumberInfos![i]);
                SubLines[i].Parent = this;
            }
        }

        private void CreateSubLines2()
        {
            for (int i = 0; i < SubLines2!.Length; i++)
            {
                SubLines2[i] = new(SubLinesStr2![i], SubLineNumberInfos2![i]);
                SubLines2[i].Parent = this;
            }
        }

        private void VerifyWriteVariableValid()
        {
            if (_Expression!._VarType == null)
            {
                if (VarToWrite!._VarType!.DefaultValue == null)
                    return;
                throw LineException.New(this,
                    $"Cannot write null to the value type variable \"{VarToWrite.Name}\"");
            }
            if (VarToWrite!._VarType != _Expression._VarType)
                throw LineException.New(this,
                    $"Cannot write expression of type {_Expression._VarType} to variable {VarToWrite.Name} of type {VarToWrite._VarType}");
        }

        private void VerifyWriteArrayElementValid()
        {
            if (_Expression!._VarType == null)
            {
                if (ElementToWrite!.Array._VarType!.Unarray!.DefaultValue == null)
                    return;
                throw LineException.New(this,
                    $"Cannot write null to a value type element of array \"{ElementToWrite.Array.Name}\"");
            }
            if (ElementToWrite!.Array._VarType!.Unarray != _Expression._VarType)
                throw LineException.New(this,
                    $"Cannot write expression of type {_Expression._VarType} to element of array " +
                    $"{ElementToWrite.Array.Name} of type {ElementToWrite.Array._VarType.Unarray}");
        }

        private Variable GetVarToWrite_NoDeclaration()
        {
            Variable? varToWrite = Memory.Instance!.GetVariable(KeyStrings[0].Text);
            if (varToWrite == null)
                throw LineException.New(this, $"Variable {KeyStrings[0].Text} hasn't been declared");
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
                throw LineException.New(this, $"Array \"{ElementToWrite.Array.Name}\" has value null");
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
            if (IsWhile)                        return Type.While;
            if (IsWhileSingleLine)              return Type.WhileSingleLine;
            if (IsWhileMultiLine)               return Type.WhileMultiLine;
            if (IsBreak)                        return Type.Break;
            if (IsContinue)                     return Type.Continue;
            if (IsFor)                          return Type.For;
            if (IsForSingleLine)                return Type.ForSingleLine;
            if (IsForMultiLine)                 return Type.ForMultiLine;
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

        private bool IsWhile
        {
            get =>
                _isWhile ??=
                    KeyStrings.Length >= 4 &&
                    KeyStrings[0]._Type == KeyString.Type.WhileKeyword &&
                    KeyStrings[1]._Type == KeyString.Type.OpenBracket &&
                    KeyStrings[KeyStrings.Length - 1]._Type == KeyString.Type.CloseBracket;
        }
        private bool? _isWhile;

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

        private bool IsWhileSingleLine
        {
            get =>
                _isWhileSingleLine ??=
                    KeyStrings.Length >= 5 &&
                    KeyStrings[0]._Type == KeyString.Type.WhileKeyword &&
                    KeyStrings[1]._Type == KeyString.Type.OpenBracket &&
                    Array.Exists(KeyStrings, keyString => keyString._Type == KeyString.Type.CloseBracket) &&
                    KeyStrings[KeyStrings.Length - 1]._Type == KeyString.Type.Semicolon;
        }
        private bool? _isWhileSingleLine;

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

        private bool IsWhileMultiLine
        {
            get =>
                _isWhileMultiLine ??=
                    KeyStrings.Length >= 6 &&
                    KeyStrings[0]._Type == KeyString.Type.WhileKeyword &&
                    KeyStrings[1]._Type == KeyString.Type.OpenBracket &&
                    Array.Exists(KeyStrings, keyString => keyString._Type == KeyString.Type.CloseBracket) &&
                    Array.Exists(KeyStrings, keyString => keyString._Type == KeyString.Type.OpenCurlyBrace) &&
                    KeyStrings[KeyStrings.Length - 1]._Type == KeyString.Type.CloseCurlyBrace;
        }
        private bool? _isWhileMultiLine;

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

        private bool IsBreak
        {
            get =>
                _isBreak ??=
                    KeyStrings.Length == 2 &&
                    KeyStrings[0]._Type == KeyString.Type.BreakKeyword &&
                    KeyStrings[1]._Type == KeyString.Type.Semicolon;
        }
        private bool? _isBreak;

        private bool IsContinue
        {
            get =>
                _isContinue ??=
                    KeyStrings.Length == 2 &&
                    KeyStrings[0]._Type == KeyString.Type.ContinueKeyword &&
                    KeyStrings[1]._Type == KeyString.Type.Semicolon;
        }
        private bool? _isContinue;

        private bool IsFor
        {
            get =>
                _isFor ??=
                    KeyStrings.Length >= 5 &&
                    KeyStrings[0]._Type == KeyString.Type.ForKeyword &&
                    KeyStrings[1]._Type == KeyString.Type.OpenBracket &&
                    Array.FindAll(KeyStrings, keyString => keyString._Type == KeyString.Type.Semicolon).Length >= 2 &&
                    KeyStrings[KeyStrings.Length - 1]._Type == KeyString.Type.CloseBracket;
        }
        private bool? _isFor;

        private bool IsForSingleLine
        {
            get =>
                _isForSingleLine ??=
                    KeyStrings.Length >= 6 &&
                    KeyStrings[0]._Type == KeyString.Type.ForKeyword &&
                    KeyStrings[1]._Type == KeyString.Type.OpenBracket &&
                    Array.FindAll(KeyStrings, keyString => keyString._Type == KeyString.Type.Semicolon).Length >= 2 &&
                    KeyStrings[KeyStrings.Length - 1]._Type == KeyString.Type.Semicolon;
        }
        private bool? _isForSingleLine;

        private bool IsForMultiLine
        {
            get =>
                _isForMultiLine ??=
                    KeyStrings.Length >= 7 &&
                    KeyStrings[0]._Type == KeyString.Type.ForKeyword &&
                    KeyStrings[1]._Type == KeyString.Type.OpenBracket &&
                    Array.FindAll(KeyStrings, keyString => keyString._Type == KeyString.Type.Semicolon).Length >= 2 &&
                    Array.Exists(KeyStrings, keyString => keyString._Type == KeyString.Type.OpenCurlyBrace) &&
                    KeyStrings[KeyStrings.Length - 1]._Type == KeyString.Type.CloseCurlyBrace;
        }
        private bool? _isForMultiLine;

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
            IfElse,
            While,
            WhileSingleLine,
            WhileMultiLine,
            For,
            ForSingleLine,
            ForMultiLine,
            Break,
            Continue
        }

        public class LineException : InterpreterException
        {
            private LineException(Line? line, string? message = null) : base(message)
            {
                this.line = line;
            }

            public Line? line;

            public static LineException New(Line? line, string? message = null, Exception? e = null)
            {
                System.Type exceptionType = e?.GetType() ?? typeof(LineException);
                string exceptionName = exceptionType.Name;
                string concatMessage = string.Empty;
                concatMessage += message is not null ? message + "\n" : string.Empty;
                concatMessage += e?.Message is not null ? e.Message : string.Empty;
                string _message = $"{exceptionName} at line {line?._LineNumberInfo.ActualLineNumber}:\n{concatMessage}";
                return new LineException(line, _message);
            }
        }
    }
}
