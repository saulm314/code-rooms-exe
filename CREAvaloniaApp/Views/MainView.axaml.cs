using Avalonia.Controls;

namespace CREAvaloniaApp.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        SetStackHeight();
    }

    private void SetStackHeight()
    {
        stackScrollViewer.Height = MainWindow.WindowSize.Height - stackHeaderTextBlock.Height - stackHeaderPanel.Height;
    }
}
