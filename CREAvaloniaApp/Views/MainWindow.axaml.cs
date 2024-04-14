using Avalonia.Controls;
using Avalonia.Media;

namespace CREAvaloniaApp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        WindowState = WindowState.Maximized;
        SetBackground();
    }

    public const byte BG = 42;
    private void SetBackground()
    {
        Color backgroundColor = new(byte.MaxValue, BG, BG, BG);
        SolidColorBrush brush = new(backgroundColor);
        Background = brush;
    }
}
