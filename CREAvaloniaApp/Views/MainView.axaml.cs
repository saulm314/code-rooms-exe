﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using CREAvaloniaApp.ViewModels;
using System.Collections.Generic;

namespace CREAvaloniaApp.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        ConfigureHeapGrid();
    }

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
    }

    public void OnEditPressed(object sender, RoutedEventArgs e)
    {
    }

    public void OnRunPressed(object sender, RoutedEventArgs e)
    {
    }

    public void OnLeftPressed(object sender, RoutedEventArgs e)
    {
    }

    public void OnRightPressed(object sender, RoutedEventArgs e)
    {
    }

    public void OnNextPressed(object sender, RoutedEventArgs e)
    {
    }
}
