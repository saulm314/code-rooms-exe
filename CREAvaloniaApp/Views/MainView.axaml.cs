using Avalonia;
using Avalonia.Controls;
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
        if (previousNewlineIndex == -1)
            return;
        int tabCount = 0;
        int i = previousNewlineIndex + 1;
        while (i < textEditorTextBox.Text.Length && textEditorTextBox.Text[i] == '\t')
        {
            tabCount++;
            i++;
        }
        List<char> newlineChars = new() { '\n' };
        for (int j = 0; j < tabCount; j++)
            newlineChars.Add('\t');
        textEditorTextBox.NewLine = new(newlineChars.ToArray());
    }
}
