using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
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
    public const int STACK_CELL_HEIGHT = 50;
    public const int STACK_LABEL_HEIGHT = 20;
    public const int STACK_SEPARATOR_HEIGHT = 15;

    public const int HEAP_HEADER_TEXT_HEIGHT = 20;
    public const int HEAP_VERTICAL_SEPARATOR_WIDTH = 5;
    public const int HEAP_HORIZONTAL_SEPARATOR_HEIGHT = 15;
    public const int HEAP_TOP_HORIZONTAL_SEPARATOR_HEIGHT = 5;
    public const int HEAP_CELL_WIDTH = 50;
    public const int HEAP_CELL_HEIGHT = 50;

    public static Size WindowSize => new(MainWindow.WindowSize.Width, MainWindow.WindowSize.Height);
    public int WindowWidth => (int)WindowSize.Width;
    public int WindowHeight => (int)WindowSize.Height;

    public static Size StackSize => new(STACK_WIDTH, WindowSize.Height);
    public int StackWidth => (int)StackSize.Width;
    public int StackHeight => (int)StackSize.Height;

    public static Size StackHeaderTextSize => new(STACK_WIDTH, STACK_HEADER_TEXT_HEIGHT);
    public int StackHeaderTextWidth => (int)StackHeaderTextSize.Width;
    public int StackHeaderTextHeight => (int)StackHeaderTextSize.Height;

    public static Size StackHeaderPanelSize => new(STACK_WIDTH, STACK_HEADER_PANEL_HEIGHT);
    public int StackHeaderPanelWidth => (int)StackHeaderPanelSize.Width;
    public int StackHeaderPanelHeight => (int)StackHeaderPanelSize.Height;

    public static Size StackScrollViewerSize => new(STACK_WIDTH, WindowSize.Height - STACK_HEADER_TEXT_HEIGHT - STACK_HEADER_PANEL_HEIGHT);
    public int StackScrollViewerWidth => (int)StackScrollViewerSize.Width;
    public int StackScrollViewerHeight => (int)StackScrollViewerSize.Height;

    public static Size HeapSize => new(HEAP_WIDTH, HEAP_HEIGHT);
    public int HeapWidth => (int)HeapSize.Width;
    public int HeapHeight => (int)HeapSize.Height;
    public GridLength HeapWidthGL => new(HeapWidth);
    public GridLength HeapHeightGL => new(HeapHeight);

    public static Size HeapHeaderTextSize => new(HEAP_WIDTH, HEAP_HEADER_TEXT_HEIGHT);
    public int HeapHeaderTextWidth => (int)HeapHeaderTextSize.Width;
    public int HeapHeaderTextHeight => (int)HeapHeaderTextSize.Height;

    public static Size HeapCellSize => new(HEAP_CELL_WIDTH, HEAP_CELL_HEIGHT);
    public int HeapCellWidth => (int)HeapCellSize.Width;
    public int HeapCellHeight => (int)HeapCellSize.Height;
    public GridLength HeapCellWidthGL => new(HeapCellWidth);
    public GridLength HeapCellHeightGL => new(HeapCellHeight);

    public static Size HeapScrollViewerSize => new(HEAP_WIDTH, HEAP_HEIGHT - HEAP_HEADER_TEXT_HEIGHT);
    public int HeapScrollViewerWidth => (int)HeapScrollViewerSize.Width;
    public int HeapScrollViewerHeight => (int)HeapScrollViewerSize.Height;

    public int HeapVerticalSeparatorWidth => HEAP_VERTICAL_SEPARATOR_WIDTH;
    public int HeapHorizontalSeparatorHeight => HEAP_HORIZONTAL_SEPARATOR_HEIGHT;
    public int HeapTopHorizontalSeparatorHeight => HEAP_TOP_HORIZONTAL_SEPARATOR_HEIGHT;
    public GridLength HeapVerticalSeparatorWidthGL => new(HeapVerticalSeparatorWidth);
    public GridLength HeapHorizontalSeparatorHeightGL => new(HeapHorizontalSeparatorHeight);
    public GridLength HeapTopHorizontalSeparatorHeightGL => new(HeapTopHorizontalSeparatorHeight);

    public static Size IdeControlsSize => new(IDE_CONTROLS_WIDTH, HEAP_HEIGHT);
    public int IdeControlsWidth => (int)IdeControlsSize.Width;
    public int IdeControlsHeight => (int)IdeControlsSize.Height;

    public static Size IdeControlSize => new(IDE_CONTROLS_WIDTH, HEAP_HEIGHT / 5);
    public int IdeControlWidth => (int)IdeControlSize.Width;
    public int IdeControlHeight => (int)IdeControlSize.Height;

    public static Size IdeControlLeftSize => new(IDE_CONTROLS_WIDTH / 2, HEAP_HEIGHT / 5);
    public int IdeControlLeftWidth => (int)IdeControlLeftSize.Width;
    public int IdeControlLeftHeight => (int)IdeControlLeftSize.Height;

    public static Size IdeControlRightSize => new(IDE_CONTROLS_WIDTH - IDE_CONTROLS_WIDTH / 2, HEAP_HEIGHT / 5);
    public int IdeControlRightWidth => (int)IdeControlRightSize.Width;
    public int IdeControlRightHeight => (int)IdeControlRightSize.Height;

    public static Size IdeControlNextSize => new(IDE_CONTROLS_WIDTH, HEAP_HEIGHT - 4 * (HEAP_HEIGHT / 5));
    public int IdeControlNextWidth => (int)IdeControlNextSize.Width;
    public int IdeControlNextHeight => (int)IdeControlNextSize.Height;

    public static Size OutputSize => new(((int)WindowSize.Width - IDE_CONTROLS_WIDTH - HEAP_WIDTH - STACK_WIDTH) / 2, HEAP_HEIGHT);
    public int OutputWidth => (int)OutputSize.Width;
    public int OutputHeight => (int)OutputSize.Height;

    public static Size DescriptionSize => new(WindowSize.Width - IDE_CONTROLS_WIDTH - HEAP_WIDTH - STACK_WIDTH - OutputSize.Width,
                                                WindowSize.Height - HEAP_HEIGHT);
    public int DescriptionWidth => (int)DescriptionSize.Width;
    public int DescriptionHeight => (int)DescriptionSize.Height;
    public GridLength DescriptionWidthGL => new(DescriptionWidth);
    public GridLength DescriptionHeightGL => new(DescriptionHeight);

    public static Size TextEditorSize => new(HEAP_WIDTH + IDE_CONTROLS_WIDTH + OutputSize.Width, DescriptionSize.Height);
    public int TextEditorWidth => (int)TextEditorSize.Width;
    public int TextEditorHeight => (int)TextEditorSize.Height;
    public GridLength TextEditorWidthGL => new(TextEditorWidth);
    public GridLength TextEditorHeightGL => new(TextEditorHeight);

    public static Size BottomRightPanelSize => new(DescriptionSize.Width, HEAP_HEIGHT);
    public int BottomRightPanelWidth => (int)BottomRightPanelSize.Width;
    public int BottomRightPanelHeight => (int)BottomRightPanelSize.Height;

    public static Size HintSize => new(DescriptionSize.Width, HEAP_HEIGHT / 4);
    public int HintWidth => (int)HintSize.Width;
    public int HintHeight => (int)HintSize.Height;

    public static Size ExitButtonsSize => new(DescriptionSize.Width, HEAP_HEIGHT - 3 * (HEAP_HEIGHT / 4));
    public int ExitButtonsWidth => (int)ExitButtonsSize.Width;
    public int ExitButtonsHeight => (int)ExitButtonsSize.Height;

    public static Size QuitButtonSize => new((int)DescriptionSize.Width / 2, ExitButtonsSize.Height);
    public int QuitButtonWidth => (int)QuitButtonSize.Width;
    public int QuitButtonHeight => (int)QuitButtonSize.Height;

    public static Size FinishButtonSize => new((int)DescriptionSize.Width - (int)DescriptionSize.Width / 2, ExitButtonsSize.Height);
    public int FinishButtonWidth => (int)FinishButtonSize.Width;
    public int FinishButtonHeight => (int)FinishButtonSize.Height;

    public const byte BG = 42;
    public const byte TEXT_EDITOR_BG = 30;
    public const byte STACK_HEADER_BG = 20;
    public const byte HEAP_HEADER_BG = 20;
    public const byte HEAP_LABEL_FG = 150;

    public IBrush WindowBrush => GetBrush(BG);
    public IBrush TextEditorBrush => GetBrush(TEXT_EDITOR_BG);
    public IBrush StackHeaderBrush => GetBrush(STACK_HEADER_BG);
    public IBrush HeapHeaderBrush => GetBrush(HEAP_HEADER_BG);
    public IBrush HeapLabelBrush => GetBrush(HEAP_LABEL_FG);

    public static SolidColorBrush GetBrush(byte brightness)
    {
        return new(new Color(byte.MaxValue, brightness, brightness, brightness));
    }
}
