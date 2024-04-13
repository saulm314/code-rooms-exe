using Avalonia.Controls;
using Avalonia.Media;

namespace CREAvaloniaApp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        WindowState = WindowState.FullScreen;
        SetBackground();
    }

    private const byte BG_A = byte.MaxValue;
    private const byte BG_R = 42;
    private const byte BG_G = 42;
    private const byte BG_B = 42;
    private void SetBackground()
    {
        Color backgroundColor = new(BG_A, BG_R, BG_G, BG_B);
        SolidColorBrush brush = new(backgroundColor);
        Background = brush;
    }
}
