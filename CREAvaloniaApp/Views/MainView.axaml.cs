using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using System.Collections.Generic;

namespace CREAvaloniaApp.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        SetStackHeaderBackground();
    }

    public const byte STACK_HEADER_BG = 30;
    private void SetStackHeaderBackground()
    {
        Color color = new(byte.MaxValue, STACK_HEADER_BG, STACK_HEADER_BG, STACK_HEADER_BG);
        SolidColorBrush brush = new(color);
        stackHeaderTextBlock.Background = brush;
    }

    public void OnTextChange(object sender, AvaloniaPropertyChangedEventArgs e)
    {
        int caretIndex = textEditorTextBox.CaretIndex;
        if (caretIndex < 1)
            return;
        if (textEditorTextBox.Text == null)
            return;
        int previousNewlineIndex = textEditorTextBox.Text.LastIndexOf('\n', caretIndex - 1);
        int tabCount = 0;
        int i = previousNewlineIndex + 1;
        while (i < textEditorTextBox.Text.Length && textEditorTextBox.Text[i] == '\t')
        {
            tabCount++;
            i++;
        }
        int previousOpenBraceIndex = textEditorTextBox.Text.LastIndexOf('{', caretIndex - 1);
        bool openedBrace;
        if (previousOpenBraceIndex == -1)
            openedBrace = false;
        else
        {
            string sub = textEditorTextBox.Text[(previousOpenBraceIndex + 1)..caretIndex];
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
        textEditorTextBox.NewLine = new(newlineChars.ToArray());
    }
}
