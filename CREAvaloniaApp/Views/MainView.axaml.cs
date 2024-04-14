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

    public const int HEAP_PAGE_SIZE = 50;

    public Panel[] HeapPanels { get; } = new Panel[HEAP_PAGE_SIZE];

    private void SetHeapGridForeground()
    {
        Color foregroundColor = new(byte.MaxValue, MainWindow.BG, MainWindow.BG, MainWindow.BG);
        SolidColorBrush brush = new(foregroundColor);
        RowDefinitions rowDefinitions = heapGrid.RowDefinitions;
        ColumnDefinitions columnDefinitions = heapGrid.ColumnDefinitions;
        int panelsAdded = 0;
        for (int i = 1; i < rowDefinitions.Count; i += 2)
            for (int j = 1; j < columnDefinitions.Count; j += 2)
            {
                Panel panel = new();
                panel[Grid.RowProperty] = i;
                panel[Grid.ColumnProperty] = j;
                panel.Background = brush;
                heapGrid.Children.Add(panel);
                HeapPanels[panelsAdded] = panel;
                panelsAdded++;
            }
    }
}
