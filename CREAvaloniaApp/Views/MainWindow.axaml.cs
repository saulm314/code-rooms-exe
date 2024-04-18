using Avalonia;
using Avalonia.Controls;
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
        double scaling = Screens.Primary!.Scaling;
        double adjustedWidth = Screens.Primary.Bounds.Width / scaling;
        double adjustedHeight = Screens.Primary.Bounds.Height / scaling;
        WindowSize = new(adjustedWidth, adjustedHeight);
        Background = MainViewModel.GetBrush(MainViewModel.BG);
    }
}
