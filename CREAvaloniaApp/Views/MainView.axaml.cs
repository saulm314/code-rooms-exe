using Avalonia.Controls;

namespace CREAvaloniaApp.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        SetResolution();
    }

    private void SetResolution()
    {
        Width = Properties.Instance.Resolution.x;
        Height = Properties.Instance.Resolution.y;
    }
}
