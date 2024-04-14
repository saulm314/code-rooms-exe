using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace CREAvaloniaApp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        WindowSize = ClientSize;
        InitializeComponent();
        WindowState = WindowState.FullScreen;
        SetBackground();
    }

    public const byte BG = 42;
    private void SetBackground()
    {
        Color backgroundColor = new(byte.MaxValue, BG, BG, BG);
        SolidColorBrush brush = new(backgroundColor);
        Background = brush;
    }

    public static Size WindowSize { get; private set; }
}
