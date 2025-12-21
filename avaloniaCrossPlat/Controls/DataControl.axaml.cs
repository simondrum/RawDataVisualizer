using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using avaloniaCrossPlat.ViewModels;

namespace avaloniaCrossPlat.Controls;

public partial class DataControl : UserControl
{

    public static readonly StyledProperty<IEnumerable<DataViewModel>> ItemsSourceProperty =
      AvaloniaProperty.Register<DataControl, IEnumerable<DataViewModel>>(nameof(ItemsSource));

    public IEnumerable<DataViewModel> ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public DataControl()
    {
        InitializeComponent();
    }
}