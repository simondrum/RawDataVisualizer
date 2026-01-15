using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;

namespace avaloniaCrossPlat.ViewModels;

public partial class DataViewModel : ViewModelBase
{
    [ObservableProperty]
    private string? _id;

    [ObservableProperty]
    private string? _name;

    [ObservableProperty]
    private decimal? _value;
    private decimal? _oldvalue;

    [ObservableProperty]
    private string? _unit;

    [ObservableProperty]
    private bool? _favorit;

    [ObservableProperty]
    private string? _title;


    [ObservableProperty]
    private BoxShadows _colorTrend;

    public DataViewModel()
    {
        ColorTrend = new BoxShadows(BoxShadow.Parse("5 5 0 0 #1f1f1f"));
        ;
    }

    partial void OnValueChanged(decimal? value)
    {
        if (value != null)
        {
            if (_oldvalue != null)
            {
                if (value < _oldvalue)
                {
                    ColorTrend = new BoxShadows(BoxShadow.Parse($@"5 5 0 0 #038387"));
                }
                else if (value > _oldvalue)
                {
                    ColorTrend = new BoxShadows(BoxShadow.Parse($@"5 5 0 0 #bc2f32"));
                }
            }
            _oldvalue = value;
        }
    }
}