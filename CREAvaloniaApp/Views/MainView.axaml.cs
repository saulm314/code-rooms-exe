using Avalonia.Controls;
using Avalonia.Media;

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
}
