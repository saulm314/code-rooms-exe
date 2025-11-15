using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using CREAvaloniaApp.ViewModels;

namespace CREAvaloniaApp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        ConfigureWindow();
        InitializeComponent();
    }

    public static Size WindowSize { get; private set; }

    private void ConfigureWindow()
    {
        WindowState = WindowState.FullScreen;
        Screen screen = Screens.ScreenFromVisual(this)!;
        double scaling = screen.Scaling;
        double adjustedWidth = screen.Bounds.Width / scaling;
        double adjustedHeight = screen.Bounds.Height / scaling;
        WindowSize = new(adjustedWidth, adjustedHeight);
        Background = MainViewModel.GetBrush(MainViewModel.BG);
    }
}
