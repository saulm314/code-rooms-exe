using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace CREAvaloniaApp.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    private void Calculate()
    {
        if (double.TryParse(celsius.Text, out double C))
        {
            double F = C * (9.0 / 5.0) + 32;
            fahrenheit.Text = F.ToString("0.0");
        }
        else
        {
            celsius.Text = "0";
            fahrenheit.Text = "0";
        }
    }

    public void ButtonClicked(object source, RoutedEventArgs args)
    {
        Calculate();
    }

    public void CelsiusUpdated(object source, AvaloniaPropertyChangedEventArgs args)
    {
        Calculate();
    }
}
