using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;


namespace avaloniaCrossPlat.Controls;

public partial class KeyPadView : UserControl
{
    public static readonly StyledProperty<string> CodeProperty =
             AvaloniaProperty.Register<KeyPadView, string>(nameof(Code), "");

    public string Code
    {
        get => GetValue(CodeProperty);
        set => SetValue(CodeProperty, value);
    }

    public KeyPadView()
    {
        InitializeComponent();
    }

    private void Button_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Content is string key)
        {
            Code += key; // contrôle lui-même le code
        }
    }
}