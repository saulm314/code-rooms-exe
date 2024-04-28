using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using CREAvaloniaApp.ViewModels;
using CRECSharpInterpreter;
using CRECSharpInterpreter.Levels;
using CRECSharpInterpreter.Levels.Tests;
using System;
using System.Collections.Generic;
using Variable = CRECSharpInterpreter.Variable;

namespace CREAvaloniaApp.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        ConfigureHeapGrid();
    }

    public Interpreter? _Interpreter { get; private set; }

    public MemoryFrame Frame => Memory.Instance!.Frames[Memory.Instance.CurrentFrame];

    private List<Panel> heapCells { get; } = new();

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
                heapCells.Add(cell);
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

    private int currentCycle = 0;
    public void OnCompilePressed(object? sender, RoutedEventArgs? e)
    {
        compileButton.IsEnabled = false;
        editButton.IsEnabled = true;
        runButton.IsEnabled = true;
        textEditor.IsReadOnly = true;
        OutputClear();
        OutputWriteLine("Compiling...");
        LevelManager.Instance.LoadLevel(1, currentCycle);
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
        catch (Exception exception)
        {
            OutputWriteLine("Error, possibly internal:");
            OutputWriteLine(exception);
            runButton.IsEnabled = false;
        }
    }

    public void OnEditPressed(object? sender, RoutedEventArgs? e)
    {
        editButton.IsEnabled = false;
        compileButton.IsEnabled = true;
        runButton.IsEnabled = false;
        leftButton.IsEnabled = false;
        rightButton.IsEnabled = false;
        nextButton.IsEnabled = false;
        textEditor.IsReadOnly = false;
        ClearStack();
        ClearHeap();
        currentCycle = 0;
    }

    public void OnRunPressed(object? sender, RoutedEventArgs? e)
    {
        runButton.IsEnabled = false;
        rightButton.IsEnabled = true;
        leftButton.IsEnabled = Frame.CanMoveLeft;
        rightButton.IsEnabled = Frame.CanMoveRight;
        DisplayFrame();
        OutputWriteLine("Running...");
    }

    public void OnLeftPressed(object? sender, RoutedEventArgs? e)
    {
        _Interpreter!.MoveLeft();
        leftButton.IsEnabled = Frame.CanMoveLeft;
        rightButton.IsEnabled = Frame.CanMoveRight;
        DisplayFrame();
    }

    public void OnRightPressed(object? sender, RoutedEventArgs? e)
    {
        _Interpreter!.MoveRight();
        leftButton.IsEnabled = Frame.CanMoveLeft;
        rightButton.IsEnabled = Frame.CanMoveRight;
        DisplayFrame();

        if (Frame.CanMoveRight)
            return;
        Declaration level = new();
        if (level.HasPassed(currentCycle))
        {
            OutputWriteLine("Pass");
            nextButton.IsEnabled = true;
            return;
        }
        OutputWriteLine("Fail");
    }

    public void OnNextPressed(object? sender, RoutedEventArgs? e)
    {
        nextButton.IsEnabled = false;
        leftButton.IsEnabled = false;
        currentCycle++;
        if (currentCycle >= LevelManager.Instance.GetCycleCount(1))
        {
            OutputWriteLine("All passed");
            return;
        }
        OutputWriteLine("Running next...");
        OnCompilePressed(null, null);
        OnRunPressed(null, null);
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

    private void DisplayFrame()
    {
        OutputClear();
        OutputWriteLine(Frame);
        DisplayStack(Frame.Stack);
        DisplayHeap(Frame.Heap);
    }

    private void DisplayStack(Stack<Scope>? stack)
    {
        ClearStack();
        if (stack == null)
            return;
        Scope[] scopes = stack.ToArray();
        for (int i = scopes.Length - 1; i >= 0; i--)
        {
            Scope scope = scopes[i];
            foreach (Variable variable in scope.DeclaredVariables)
                PushToStack(variable);
        }
    }

    private void PushToStack(Variable variable)
    {
        Image image = new()
        {
            Height = MainViewModel.STACK_CELL_HEIGHT,
            Width = MainViewModel.STACK_CELL_HEIGHT,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Source = new Bitmap(@$"..\..\..\..\Files\Types\{variable._VarType}.png")
        };
        TextBlock name = new()
        {
            Text = variable.Name,
            FontSize = 16,
            FontFamily = new("Cascadia Mono"),
            Foreground = new SolidColorBrush(Colors.White),
            TextAlignment = TextAlignment.Center,
            Height = MainViewModel.STACK_LABEL_HEIGHT,
            Width = MainViewModel.STACK_WIDTH
        };
        TextBlock value = new()
        {
            Text = variable.ValueAsString,
            FontSize = 16,
            FontFamily = new("Cascadia Mono"),
            FontWeight = FontWeight.UltraBold,
            Foreground = new SolidColorBrush(Colors.Black),
            TextAlignment = TextAlignment.Center,
            Height = 20,
            Width = MainViewModel.STACK_CELL_HEIGHT,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        Panel imageBox = new()
        {
            Height = MainViewModel.STACK_CELL_HEIGHT,
            Width = MainViewModel.STACK_CELL_HEIGHT
        };
        Panel separator = new()
        {
            Height = MainViewModel.STACK_SEPARATOR_HEIGHT,
            Width = MainViewModel.STACK_WIDTH
        };
        imageBox.Children.Add(image);
        imageBox.Children.Add(value);
        stackPanel.Children.Add(imageBox);
        stackPanel.Children.Add(name);
        stackPanel.Children.Add(separator);
    }

    private void ClearStack()
    {
        stackPanel.Children.Clear();
    }

    private void ClearHeap()
    {
        foreach (Panel panel in heapCells)
            panel.Children.Clear();
    }

    private void DisplayHeap(Heap? heap)
    {
        if (heap == null)
        {
            ClearHeap();
            return;
        }
        for (int i = 0; i < 50; i++)
        {
            heapCells[i].Children.Clear();
            Variable? variable = heap[i];
            if (variable?._VarType == null)
                continue;
            Image image = new()
            {
                Height = MainViewModel.STACK_CELL_HEIGHT,
                Width = MainViewModel.STACK_CELL_HEIGHT,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Source = new Bitmap(@$"..\..\..\..\Files\Types\{variable._VarType}.png")
            };
            TextBlock value = new()
            {
                Text = variable.ValueAsString,
                FontSize = 16,
                FontFamily = new("Cascadia Mono"),
                FontWeight = FontWeight.UltraBold,
                Foreground = new SolidColorBrush(Colors.Black),
                TextAlignment = TextAlignment.Center,
                Height = 20,
                Width = MainViewModel.STACK_CELL_HEIGHT,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            heapCells[i].Children.Add(image);
            heapCells[i].Children.Add(value);
        }
    }
}
