using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using CREAvaloniaApp.ViewModels;
using CRECSharpInterpreter;
using System.Collections.Generic;

namespace CREAvaloniaApp.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        ConfigureHeapGrid();
    }

    public Interpreter? _Interpreter { get; private set; }

    private void ConfigureHeapGrid()
    {
        RowDefinitions rowDefinitions = heapGrid.RowDefinitions;
        ColumnDefinitions columnDefinitions = heapGrid.ColumnDefinitions;
        int count = 0;
        for (int i = 1; i < rowDefinitions.Count; i += 2)
        {
            for (int j = 1; j < columnDefinitions.Count; j += 2)
            {
                Panel cell = new()
                {
                    Background = MainViewModel.GetBrush(MainViewModel.BG),
                    [Grid.RowProperty] = i,
                    [Grid.ColumnProperty] = j
                };
                heapGrid.Children.Add(cell);
                TextBlock label = new()
                {
                    Text = count == 0 ? "null" : count.ToString(),
                    FontSize = 13,
                    FontFamily = new("Cascadia Mono"),
                    Foreground = MainViewModel.GetBrush(MainViewModel.HEAP_LABEL_FG),
                    FontWeight = FontWeight.UltraBold,
                    TextAlignment = TextAlignment.Center,
                    [Grid.RowProperty] = i + 1,
                    [Grid.ColumnProperty] = j
                };
                heapGrid.Children.Add(label);
                count++;
            }
        }
    }

    public void OnTextChange(object sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (textEditor?.Text == null)
            return;
        int caretIndex = textEditor.CaretIndex;
        if (caretIndex < 1)
            return;
        int previousNewlineIndex = textEditor.Text.LastIndexOf('\n', caretIndex - 1);
        int tabCount = 0;
        int i = previousNewlineIndex + 1;
        while (i < textEditor.Text.Length && textEditor.Text[i] == '\t')
        {
            tabCount++;
            i++;
        }
        int previousOpenBraceIndex = textEditor.Text.LastIndexOf('{', caretIndex - 1);
        bool openedBrace;
        if (previousOpenBraceIndex == -1)
            openedBrace = false;
        else
        {
            string sub = textEditor.Text[(previousOpenBraceIndex + 1)..caretIndex];
            if (!string.IsNullOrWhiteSpace(sub))
                openedBrace = false;
            else if (sub.Contains('\n'))
                openedBrace = false;
            else
                openedBrace = true;
        }
        List<char> newlineChars = new() { '\n' };
        for (int j = 0; j < tabCount; j++)
            newlineChars.Add('\t');
        if (openedBrace)
        {
            newlineChars.Add('\t');
            newlineChars.Add('\n');
            for (int j = 0; j < tabCount; j++)
                newlineChars.Add('\t');
            newlineChars.Add('}');
        }
        textEditor.NewLine = new(newlineChars.ToArray());
    }

    public void OnCompilePressed(object sender, RoutedEventArgs e)
    {
        compileButton.IsEnabled = false;
        editButton.IsEnabled = true;
        runButton.IsEnabled = true;
        textEditor.IsReadOnly = true;
        arrowCount = 0;
        maxArrowCount = 0;
        executed = false;
        thrown = false;
        statements = new();
        interpreterException = null;
        OutputClear();
        OutputWriteLine("Compiling...");
        try
        {
            _Interpreter = new Interpreter(textEditor.Text ?? string.Empty);
            OutputWriteLine("Compilation successful");
        }
        catch (InterpreterException exception)
        {
            OutputWriteLine(exception.Message);
            runButton.IsEnabled = false;
        }
    }

    public void OnEditPressed(object sender, RoutedEventArgs e)
    {
        editButton.IsEnabled = false;
        compileButton.IsEnabled = true;
        runButton.IsEnabled = false;
        leftButton.IsEnabled = false;
        rightButton.IsEnabled = false;
        nextButton.IsEnabled = false;
        textEditor.IsReadOnly = false;
    }

    public void OnRunPressed(object sender, RoutedEventArgs e)
    {
        runButton.IsEnabled = false;
        rightButton.IsEnabled = true;
        OutputWriteLine("Running...");
    }

    private int arrowCount = 0;
    private int maxArrowCount = 0;
    private bool executed = false;
    private bool thrown = false;
    private InterpreterException? interpreterException;
    private List<Statement>? statements;

    public void OnLeftPressed(object sender, RoutedEventArgs e)
    {
        if (arrowCount == 1)
            leftButton.IsEnabled = false;
        rightButton.IsEnabled = true;
        arrowCount--;
        OutputClear();
        if (arrowCount > 0 && arrowCount - 1 < statements!.Count)
            OutputWriteLine(statements[arrowCount - 1]);
    }

    public void OnRightPressed(object sender, RoutedEventArgs e)
    {
        leftButton.IsEnabled = true;
        if (arrowCount < maxArrowCount)
        {
            if (arrowCount < statements!.Count)
            {
                OutputClear();
                OutputWriteLine(statements[arrowCount]);
            }
            arrowCount++;
            if (arrowCount == maxArrowCount && (executed || thrown))
            {
                OutputClear();
                if (thrown)
                {
                    OutputWriteLine(statements[arrowCount - 1] + "\n");
                    OutputWriteLine(interpreterException!.Message);
                }
                rightButton.IsEnabled = false;
            }
            return;
        }
        int statementNumber = _Interpreter!.chunk.statementsDone;
        try
        {
            executed = !_Interpreter.chunk.RunNextStatement();
            if (statementNumber < _Interpreter.chunk.Statements.Length)
            {
                Statement statement = _Interpreter.chunk.Statements[statementNumber];
                statements!.Add(statement);
                OutputClear();
                OutputWriteLine(statements[arrowCount]);
            }
            if (executed)
            {
                rightButton.IsEnabled = false;
                OutputClear();
                OutputWriteLine("Execution finished");
                executed = true;
                nextButton.IsEnabled = true;
            }
            arrowCount++;
            maxArrowCount++;
        }
        catch (InterpreterException exception)
        {
            Statement statement = _Interpreter.chunk.Statements[statementNumber];
            statements!.Add(statement);
            OutputClear();
            OutputWriteLine(_Interpreter!.chunk.Statements[_Interpreter.chunk.statementsDone] + "\n");
            OutputWriteLine(exception.Message);
            interpreterException = exception;
            rightButton.IsEnabled = false;
            thrown = true;
            arrowCount++;
            maxArrowCount++;
        }
    }

    public void OnNextPressed(object sender, RoutedEventArgs e)
    {
        nextButton.IsEnabled = false;
        leftButton.IsEnabled = false;
        OutputWriteLine("Running next...");
    }

    public void OutputWriteLine(object message)
    {
        output.Text += message + "\n";
    }

    public void OutputWrite(object message)
    {
        output.Text += message;
    }

    public void OutputClear()
    {
        output.Clear();
    }
}
