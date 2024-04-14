using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace CREAvaloniaApp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        ConfigureWindow();
        SetBackground();
        InitializeComponent();
    }

    public const byte BG = 42;
    private void SetBackground()
    {
        Color backgroundColor = new(byte.MaxValue, BG, BG, BG);
        SolidColorBrush brush = new(backgroundColor);
        Background = brush;
    }

    public static Size WindowSize { get; private set; }

    private void ConfigureWindow()
    {
        WindowState = WindowState.FullScreen;
        double scaling = Screens.Primary!.Scaling;
        double adjustedWidth = Screens.Primary.Bounds.Width / scaling;
        double adjustedHeight = Screens.Primary.Bounds.Height / scaling;
        WindowSize = new(adjustedWidth, adjustedHeight);
    }
}
