﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using CREAvaloniaApp.ViewModels;
using CRECSharpInterpreter;
using CRECSharpInterpreter.Levels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Environment = CRECSharpInterpreter.Environment;
using Variable = CRECSharpInterpreter.Variable;

namespace CREAvaloniaApp.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        ConfigureHeapGrid();

        LoadLevel(0);
        OnSyntaxPressed(null, null);
        OnSyntaxPressed(null, null);

        save = GetSave();
        UpdateFromSave();
    }

    private void LoadLevel(int id, int cycle = 0)
    {
        LevelManager.Instance.LoadLevel(id, cycle);
        Level level = LevelManager.Instance.GetLevel(id);
        description.Text = level.Description ?? string.Empty;
    }

    private Save GetSave()
    {
        #if DEBUG
        string[] files = Directory.GetFiles(@"..\..\..\..\Files\Save");
        #elif RELEASE
        string[] files = Directory.GetFiles(@"Files\Save");
        #endif
        string? file = Array.Find(files, file => file.EndsWith("player.json"));
        Save save;
        try
        {
            save = JsonConvert.DeserializeObject<Save>(File.ReadAllText(file!))!;
            if (save == null)
                throw new ArgumentNullException();
        }
        catch (Exception e) when (e is JsonException or ArgumentNullException)
        {
            save = new();
            string jsonText = JsonConvert.SerializeObject(save, Formatting.Indented);
            #if DEBUG
            string filePath = @"..\..\..\..\Files\Save\player.json";
            #elif RELEASE
            string filePath = @"Files\Save\player.json";
            #endif
            File.Create(filePath).Close();
            File.WriteAllText(filePath, jsonText);
        }
        return save;
    }

    private Save save;

    private void UpdateFromSave()
    {
        bool zeroReached = false;
        Button[] levelButtons = GetLevelButtons();
        TextBlock[] levelTextBlocks = GetLevelTextBlocks();
        for (int i = 0; i < levelButtons.Length; i++)
        {
            Level? level = i + 1 < LevelManager.Instance.Levels.Length ? LevelManager.Instance.GetLevel(i + 1) : null;
            int starsCollected = i < save.starsCollected!.Length ? save.starsCollected[i] : 0;
            levelTextBlocks[i].Text = $"Level {i + 1}: {level?.name}\n         {starsCollected}/{level?.maxStars} ⭐";
            if (zeroReached)
            {
                levelButtons[i].IsEnabled = false;
                continue;
            }
            if (i >= save.starsCollected.Length)
            {
                zeroReached = true;
                levelButtons[i].IsEnabled = true;
                continue;
            }
            if (save.starsCollected[i] == 0)
            {
                zeroReached = true;
                levelButtons[i].IsEnabled = true;
                continue;
            }
            levelButtons[i].IsEnabled = true;
        }
        int totalAcquired = save.starsCollected!.Sum();
        int totalAvailable = LevelManager.Instance.Levels.Sum(level => level.maxStars);
        totalText.Text = $"Total:\n{totalAcquired}/{totalAvailable} ⭐";
    }

    private void UpdateSaveFile()
    {
        string jsonText = JsonConvert.SerializeObject(save, Formatting.Indented);
        #if DEBUG
        string filePath = @"..\..\..\..\Files\Save\player.json";
        #elif RELEASE
        string filePath = @"Files\Save\player.json";
        #endif
        File.Create(filePath).Close();
        File.WriteAllText(filePath, jsonText);
    }

    private Button[] GetLevelButtons()
    {
        return
        [
            level1Button,
            level2Button,
            level3Button,
            level4Button,
            level5Button,
            level6Button,
            level7Button,
            level8Button,
            level9Button
        ];
    }

    private TextBlock[] GetLevelTextBlocks()
    {
        return
        [
            level1Text,
            level2Text,
            level3Text,
            level4Text,
            level5Text,
            level6Text,
            level7Text,
            level8Text,
            level9Text
        ];
    }

    public Interpreter? _Interpreter { get; private set; }

    public MemoryFrame Frame => Memory.Instance!.Frames[Memory.Instance.CurrentFrame];

    private List<Panel> heapCells { get; } = new();

    private void ConfigureHeapGrid()
    {
        heapGrid.Children.Clear();
        heapCells.Clear();
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
    private int minStars = int.MaxValue;
    public void OnCompilePressed(object? sender, RoutedEventArgs? e)
    {
        compileButton.IsEnabled = false;
        editButton.IsEnabled = true;
        runButton.IsEnabled = true;
        syntaxButton.IsEnabled = false;
        textEditor.IsReadOnly = true;
        OutputClear();
        OutputWriteLine("Compiling...");
        LoadLevel(LevelManager.Instance.CurrentLevel, currentCycle);
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
        syntaxButton.IsEnabled = true;
        textEditor.IsReadOnly = false;
        ClearStack();
        ClearHeap();
        currentCycle = 0;
        minStars = int.MaxValue;
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
        ILevelTest levelTest = LevelManager.Instance.GetLevelTest();
        int starsAchieved = levelTest.StarsAchieved(currentCycle);
        if (starsAchieved > 0)
        {
            OutputWriteLine($"Pass with {starsAchieved} stars");
            nextButton.IsEnabled = true;
            minStars = starsAchieved < minStars ? starsAchieved : minStars;
            return;
        }
        if (LevelManager.Instance.CurrentLevel != 0)
            OutputWriteLine("Fail");
    }

    public void OnNextPressed(object? sender, RoutedEventArgs? e)
    {
        nextButton.IsEnabled = false;
        leftButton.IsEnabled = false;
        currentCycle++;
        if (currentCycle >= LevelManager.Instance.GetCycleCount())
        {
            OutputWriteLine($"All passed with {minStars} stars; next level unlocked");
            int current = LevelManager.Instance.CurrentLevel;
            save.starsCollected![current - 1] = save.starsCollected[current - 1] < minStars ? minStars : save.starsCollected[current - 1];
            UpdateSaveFile();
            UpdateFromSave();
            return;
        }
        OutputWriteLine("Running next...");
        OnCompilePressed(null, null);
        OnRunPressed(null, null);
    }

    public void OnLevel0Pressed(object? sender, RoutedEventArgs? e)
    {
        textEditor.Text = string.Empty;
        OnEditPressed(null, null);
        LoadLevel(0);
    }

    public void OnLevel1Pressed(object? sender, RoutedEventArgs? e)
    {
        textEditor.Text = string.Empty;
        OnEditPressed(null, null);
        LoadLevel(1);
    }

    public void OnLevel2Pressed(object? sender, RoutedEventArgs? e)
    {
        textEditor.Text = string.Empty;
        OnEditPressed(null, null);
        LoadLevel(2);
    }

    public void OnLevel3Pressed(object? sender, RoutedEventArgs? e)
    {
        textEditor.Text = string.Empty;
        OnEditPressed(null, null);
        LoadLevel(3);
    }

    public void OnLevel4Pressed(object? sender, RoutedEventArgs? e)
    {
        textEditor.Text = string.Empty;
        OnEditPressed(null, null);
        LoadLevel(4);
    }

    public void OnLevel5Pressed(object? sender, RoutedEventArgs? e)
    {
        textEditor.Text = string.Empty;
        OnEditPressed(null, null);
        LoadLevel(5);
    }

    public void OnLevel6Pressed(object? sender, RoutedEventArgs? e)
    {
        textEditor.Text = string.Empty;
        OnEditPressed(null, null);
        LoadLevel(6);
    }

    public void OnLevel7Pressed(object? sender, RoutedEventArgs? e)
    {
        textEditor.Text = string.Empty;
        OnEditPressed(null, null);
        LoadLevel(7);
    }

    public void OnLevel8Pressed(object? sender, RoutedEventArgs? e)
    {
        textEditor.Text = string.Empty;
        OnEditPressed(null, null);
        LoadLevel(8);
    }

    public void OnLevel9Pressed(object? sender, RoutedEventArgs? e)
    {
        textEditor.Text = string.Empty;
        OnEditPressed(null, null);
        LoadLevel(9);
    }

    public void OnSyntaxPressed(object? sender, RoutedEventArgs? e)
    {
        Environment._Syntax = Environment._Syntax switch
        {
            Syntax.CSharp => Syntax.Java,
            Syntax.Java => Syntax.CSharp,
            _ => Syntax.CSharp
        };
        string baseText = "Current Syntax:\n";
        string result = Environment._Syntax switch
        {
            Syntax.CSharp => "C#",
            Syntax.Java => "Java",
            _ => string.Empty
        };
        syntaxText.Text = baseText + result;
    }

    public void OnQuitPressed(object? sender, RoutedEventArgs? e)
    {
        Application application = Application.Current!;
        ClassicDesktopStyleApplicationLifetime applicationLifetime = (ClassicDesktopStyleApplicationLifetime)application.ApplicationLifetime!;
        applicationLifetime.Shutdown();
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
        DisplayStack(Frame.Stack, Frame.ActiveLoops, Frame.ActiveForHeaders);
        DisplayHeap(Frame.Heap);
    }

    private void DisplayStack(Stack<Scope>? stack, Stack<int>? activeLoops, Stack<int>? activeForHeaders)
    {
        ClearStack();
        if (stack == null || activeLoops == null || activeForHeaders == null)
            return;
        Scope[] scopes = stack.ToArray();
        for (int i = scopes.Length - 1; i >= 0; i--)
        {
            Scope scope = scopes[i];
            foreach (Variable variable in scope.DeclaredVariables)
                PushToStack(variable);

            if (i == 0)
                continue;

            int antiIndex = scopes.Length - 1 - i;
            bool scopeEndIsActiveForHeader = activeForHeaders.Contains(antiIndex + 1);
            bool previousScopeEndIsActiveForHeader = activeForHeaders.Contains(antiIndex);
            bool scopeEndIsActiveLoop = activeLoops.Contains(antiIndex + 1);
            IBrush separatorBrush = (scopeEndIsActiveForHeader, previousScopeEndIsActiveForHeader, scopeEndIsActiveLoop) switch
            {
                (true, _, _) => MainViewModel.GetBrush(MainViewModel.STACK_LOOP_SEPARATOR_BG),
                (false, true, _) => MainViewModel.GetBrush(MainViewModel.STACK_FOR_HEADER_SEPARATOR_BG),
                (false, false, true) => MainViewModel.GetBrush(MainViewModel.STACK_LOOP_SEPARATOR_BG),
                _ => MainViewModel.GetBrush(MainViewModel.STACK_NON_LOOP_SEPARATOR_BG)
            };

            // the indexer for the Controls class appears to be broken,
            //      so to get the last element, we enumerate through the entire collection
            //      instead of simply referencing stackPanel.Children[^1]
            // in practice there won't be very many items in this list, so this shouldn't cause performance issues
            int lastChildIndex = stackPanel.Children.Count - 1;
            int j = 0;
            foreach (Control control in stackPanel.Children)
            {
                if (j++ != lastChildIndex)
                    continue;
                Panel panel = (Panel)control;
                Panel childPanel = new()
                {
                    Height = MainViewModel.STACK_SEPARATOR_HEIGHT / 2,
                    Width = MainViewModel.STACK_WIDTH
                };
                childPanel.Background = separatorBrush;
                panel.Children.Add(childPanel);
            }
        }
        stackScroll.ScrollToEnd();
    }

    private void PushToStack(Variable variable)
    {
        Image image = new()
        {
            Height = MainViewModel.STACK_CELL_HEIGHT,
            Width = MainViewModel.STACK_CELL_HEIGHT,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            #if DEBUG
            Source = new Bitmap(@$"..\..\..\..\Files\Types\{variable._VarType}.png")
            #elif RELEASE
            Source = new Bitmap(@$"Files\Types\{variable._VarType}.png")
            #endif
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
            FontSize = GetFontSize(variable),
            FontFamily = new("Cascadia Mono"),
            FontWeight = GetFontWeight(variable),
            Foreground = GetForeground(variable),
            TextAlignment = TextAlignment.Center,
            Height = GetHeight(variable),
            Width = GetWidth(variable),
            HorizontalAlignment = GetHorizontalAlignment(variable),
            VerticalAlignment = GetVerticalAlignment(variable)
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

    private int GetFontSize(Variable variable)
    {
        int length = variable.ValueAsString.Length;
        return variable._VarType!.Slug switch
        {
            "bool" => 16,
            "char" => 16,
            "int" or "double" when length <= 4 => 16,
            "int" or "double" when length <= 5 => 12,
            "int" or "double" => 8,
            "string" when length <= 3 => 16,
            "string" when length <= 4 => 12,
            "string" => 8,
            _ when length <= 4 => 16,
            _ when length <= 6 => 12,
            _ => 8
        };
    }

    private int GetHeight(Variable variable)
    {
        int length = variable.ValueAsString.Length;
        return variable._VarType!.Slug switch
        {
            "int" or "double" when length <= 5 => 20,
            "int" or "double" => 15,
            _ => 20
        };
    }

    private int GetWidth(Variable variable)
    {
        return variable._VarType!.Slug switch
        {
            "string" => MainViewModel.STACK_CELL_HEIGHT * 9 / 10,
            _ => MainViewModel.STACK_CELL_HEIGHT
        };
    }

    private FontWeight GetFontWeight(Variable variable)
    {
        return variable._VarType!._Storage == VarType.Storage.Value ?
            FontWeight.UltraBold :
            FontWeight.Normal;
    }

    private IBrush GetForeground(Variable variable)
    {
        return variable._VarType!._Storage == VarType.Storage.Value ?
            new SolidColorBrush(Colors.Black) :
            new SolidColorBrush(Colors.White);
    }

    private HorizontalAlignment GetHorizontalAlignment(Variable variable)
    {
        return variable._VarType!.Slug switch
        {
            "string" => HorizontalAlignment.Right,
            _ => HorizontalAlignment.Center
        };
    }

    private VerticalAlignment GetVerticalAlignment(Variable variable)
    {
        int length = variable.ValueAsString.Length;
        return variable._VarType!.Slug switch
        {
            "int" or "double" when length <= 2 => VerticalAlignment.Center,
            "int" or "double" => VerticalAlignment.Bottom,
            _ => VerticalAlignment.Center
        };
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

    private int displayedHeapSize = 50;
    private void DisplayHeap(Heap? heap)
    {
        if (heap == null)
        {
            ClearHeap();
            return;
        }
        int previousDisplayedHeapSize = displayedHeapSize;
        while (displayedHeapSize < heap.Size)
        {
            for (int i = 0; i < 5; i++)
            {
                RowDefinition cellRow = new()
                {
                    Height = new(MainViewModel.HEAP_CELL_HEIGHT)
                };
                RowDefinition separatorRow = new()
                {
                    Height = new(MainViewModel.HEAP_HORIZONTAL_SEPARATOR_HEIGHT)
                };
                heapGrid.RowDefinitions.Add(cellRow);
                heapGrid.RowDefinitions.Add(separatorRow);
            }
            ConfigureHeapGrid();
            displayedHeapSize += 50;
        }
        while (displayedHeapSize > heap.Size)
        {
            for (int i = 0; i < 5; i++)
            {
                heapGrid.RowDefinitions.RemoveAt(heapGrid.RowDefinitions.Count - 1);
                heapGrid.RowDefinitions.RemoveAt(heapGrid.RowDefinitions.Count - 1);
            }
            ConfigureHeapGrid();
            displayedHeapSize -= 50;
        }
        for (int i = 0; i < displayedHeapSize; i++)
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
                #if DEBUG
                Source = new Bitmap(@$"..\..\..\..\Files\Types\{variable._VarType}.png")
                #elif RELEASE
                Source = new Bitmap(@$"Files\Types\{variable._VarType}.png")
                #endif
            };
            TextBlock value = new()
            {
                Text = variable.ValueAsString,
                FontSize = GetFontSize(variable),
                FontFamily = new("Cascadia Mono"),
                FontWeight = GetFontWeight(variable),
                Foreground = GetForeground(variable),
                TextAlignment = TextAlignment.Center,
                Height = GetHeight(variable),
                Width = GetWidth(variable),
                HorizontalAlignment = GetHorizontalAlignment(variable),
                VerticalAlignment = GetVerticalAlignment(variable)
            };
            heapCells[i].Children.Add(image);
            heapCells[i].Children.Add(value);
        }
        if (displayedHeapSize != previousDisplayedHeapSize)
            heapScroll.ScrollToEnd();
    }
}
