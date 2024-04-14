using Avalonia;
using CREAvaloniaApp.Views;

namespace CREAvaloniaApp.ViewModels;

public class MainViewModel : ViewModelBase
{
    public const int STACK_WIDTH = 130;
    public const int HEAP_WIDTH = 555;
    public const int HEAP_HEIGHT = 350;
    public const int IDE_CONTROLS_WIDTH = 100;

    public const int STACK_HEADER_TEXT_HEIGHT = 20;
    public const int STACK_HEADER_PANEL_HEIGHT = 10;
    public static int s_StackScrollViewerHeight => (int)MainWindow.WindowSize.Height - STACK_HEADER_TEXT_HEIGHT - STACK_HEADER_PANEL_HEIGHT;

    public const int HEAP_HEADER_TEXT_HEIGHT = 20;
    public const int HEAP_VERTICAL_SEPARATOR_WIDTH = 5;
    public const int HEAP_HORIZONTAL_SEPARATOR_HEIGHT = 15;
    public const int HEAP_TOP_HORIZONTAL_SEPARATOR_HEIGHT = 5;
    public const int HEAP_CELL_WIDTH = 50;
    public const int HEAP_CELL_HEIGHT = 50;

    public int StackWidth => STACK_WIDTH;
    public int HeapWidth => HEAP_WIDTH;
    public int HeapHeight => HEAP_HEIGHT;
    public int IdeControlsWidth => IDE_CONTROLS_WIDTH;
    
    public int StackHeaderTextHeight => STACK_HEADER_TEXT_HEIGHT;
    public int StackHeaderPanelHeight => STACK_HEADER_PANEL_HEIGHT;
    public int StackScrollViewerHeight => s_StackScrollViewerHeight;
    
    public int HeapHeaderTextHeight => HEAP_HEADER_TEXT_HEIGHT;
    public int HeapVerticalSeparatorWidth => HEAP_VERTICAL_SEPARATOR_WIDTH;
    public int HeapHorizontalSeparatorHeight => HEAP_HORIZONTAL_SEPARATOR_HEIGHT;
    public int HeapTopHorizontalSeparatorHeight => HEAP_TOP_HORIZONTAL_SEPARATOR_HEIGHT;
    public int HeapCellWidth => HEAP_CELL_WIDTH;
    public int HeapCellHeight => HEAP_CELL_HEIGHT;

    public static Size StackSize => new(STACK_WIDTH, MainWindow.WindowSize.Height);
    public static Size HeapSize => new(HEAP_WIDTH, HEAP_HEIGHT);
    public static Size IdeControlsSize => new(IDE_CONTROLS_WIDTH, HEAP_HEIGHT);
    public static Size IdeControlSize => new(IDE_CONTROLS_WIDTH, HEAP_HEIGHT / 5);
    public static Size IdeControlLeftSize => new(IDE_CONTROLS_WIDTH / 2, HEAP_HEIGHT / 5);
    public static Size IdeControlRightSize => new(IDE_CONTROLS_WIDTH - IDE_CONTROLS_WIDTH / 2, HEAP_HEIGHT / 5);
    public static Size IdeControlNextSize => new(IDE_CONTROLS_WIDTH, HEAP_HEIGHT - 4 * (HEAP_HEIGHT / 5));
    public static Size OutputSize => new(((int)MainWindow.WindowSize.Width - IDE_CONTROLS_WIDTH - HEAP_WIDTH - STACK_WIDTH) / 2, HEAP_HEIGHT);
    public static Size DescriptionSize => new(MainWindow.WindowSize.Width - IDE_CONTROLS_WIDTH - HEAP_WIDTH - STACK_WIDTH - OutputSize.Width,
                                                MainWindow.WindowSize.Height - HEAP_HEIGHT);
    public static Size TextEditorSize => new(HEAP_WIDTH + IDE_CONTROLS_WIDTH + OutputSize.Width, DescriptionSize.Height);
    public static Size BottomRightPanelSize => new(DescriptionSize.Width, HEAP_HEIGHT);
    public static Size HintSize => new(DescriptionSize.Width, HEAP_HEIGHT / 4);
    public static Size ExitButtonsSize => new(DescriptionSize.Width, HEAP_HEIGHT - 3 * (HEAP_HEIGHT / 4));
    public static Size QuitButtonSize => new((int)DescriptionSize.Width / 2, ExitButtonsSize.Height);
    public static Size FinishButtonSize => new((int)DescriptionSize.Width - (int)DescriptionSize.Width / 2, ExitButtonsSize.Height);
}
