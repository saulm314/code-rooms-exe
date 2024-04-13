using Avalonia.Controls;
using Avalonia.Media;

namespace CREAvaloniaApp.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        SetHeapGridBackground();
    }

    private const byte HEAP_GRID_BG_A = byte.MaxValue;
    private const byte HEAP_GRID_BG_R = 100;
    private const byte HEAP_GRID_BG_G = 100;
    private const byte HEAP_GRID_BG_B = 100;
    private void SetHeapGridBackground()
    {
        Color backgroundColor = new(HEAP_GRID_BG_A, HEAP_GRID_BG_R, HEAP_GRID_BG_G, HEAP_GRID_BG_B);
        SolidColorBrush brush = new(backgroundColor);
        heapGrid.Background = brush;
    }
}
