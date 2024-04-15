using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using System.Collections.Generic;

namespace CREAvaloniaApp.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
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
}
