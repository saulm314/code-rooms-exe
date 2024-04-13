using Avalonia.Controls;
using Avalonia.Media;

namespace CREAvaloniaApp.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        SetHeapGridBackground();
        SetHeapGridForeground();
    }

    public const byte HEAP_GRID_BG = 100;
    private void SetHeapGridBackground()
    {
        Color backgroundColor = new(byte.MaxValue, HEAP_GRID_BG, HEAP_GRID_BG, HEAP_GRID_BG);
        SolidColorBrush brush = new(backgroundColor);
        heapGrid.Background = brush;
    }

    private void SetHeapGridForeground()
    {
        Color foregroundColor = new(byte.MaxValue, MainWindow.BG, MainWindow.BG, MainWindow.BG);
        SolidColorBrush brush = new(foregroundColor);
        ColumnDefinitions columnDefinitions = heapGrid.ColumnDefinitions;
        RowDefinitions rowDefinitions = heapGrid.RowDefinitions;
        for (int i = 1; i < columnDefinitions.Count; i += 2)
            for (int j = 0; j < rowDefinitions.Count; j += 2)
            {
                Panel panel = new();
                panel[Grid.ColumnProperty] = i;
                panel[Grid.RowProperty] = j;
                panel.Background = brush;
                heapGrid.Children.Add(panel);
            }
    }
}
