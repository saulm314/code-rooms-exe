using Avalonia.Controls;
using Avalonia.Media;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;

namespace CREAvaloniaApp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        ReadProperties();
        InitializeComponent();
        WindowState = WindowState.FullScreen;
        SetBackground();
    }

    private const string PROPERTIES_PATH = @"..\..\..\..\Files\CREProperties.json";
    private void ReadProperties()
    {
        try
        {
            Properties.Instance = JsonConvert.DeserializeObject<Properties>(File.ReadAllText(PROPERTIES_PATH)) ?? throw new Exception();
        }
        catch
        {
            Debug.WriteLine("Failed to read properties from JSON file; using default properties");
            Properties.Instance = new();
        }
        Debug.WriteLine($"Resolution detected: {Properties.Instance.Resolution.x}x{Properties.Instance.Resolution.y}");
    }

    public const byte BG = 42;
    private void SetBackground()
    {
        Color backgroundColor = new(byte.MaxValue, BG, BG, BG);
        SolidColorBrush brush = new(backgroundColor);
        Background = brush;
    }
}
