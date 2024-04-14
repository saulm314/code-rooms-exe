using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace CREAvaloniaApp.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        SetHeapGridBackground();
        SetHeapGridForeground();
        SetHeapIndexes();
    }

    public const byte HEAP_GRID_BG = 0;
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
                Panel panel = new()
                {
                    Background = brush,
                    [Grid.RowProperty] = i,
                    [Grid.ColumnProperty] = j
                };
                heapGrid.Children.Add(panel);
                HeapPanels[panelsAdded] = panel;
                panelsAdded++;
            }
    }

    public TextBlock[] HeapIndexTextBlocks { get; } = new TextBlock[HEAP_PAGE_SIZE];

    public const byte HEAP_INDEX_FG = HEAP_GRID_BG;
    private void SetHeapIndexes()
    {
        Color foregroundColor = new(byte.MaxValue, 150, 150, 150);
        SolidColorBrush foregroundBrush = new(foregroundColor);
        RowDefinitions rowDefinitions = heapGrid.RowDefinitions;
        ColumnDefinitions columnDefinitions = heapGrid.ColumnDefinitions;
        int count = 0;
        for (int i = 2; i < rowDefinitions.Count; i += 2)
            for (int j = 1; j < columnDefinitions.Count; j += 2)
            {
                TextBlock textBlock = new()
                {
                    Text = count == 0 ? "null" : count.ToString(),
                    FontSize = 13,
                    FontFamily = new("Cascadia Mono"),
                    Foreground = foregroundBrush,
                    FontWeight = FontWeight.UltraBold,
                    TextAlignment = TextAlignment.Center,
                    [Grid.RowProperty] = i,
                    [Grid.ColumnProperty] = j
                };
                heapGrid.Children.Add(textBlock);
                HeapIndexTextBlocks[count] = textBlock;
                count++;
            }
    }
}
