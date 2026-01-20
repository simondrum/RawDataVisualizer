using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Avalonia.Markup.Xaml.MarkupExtensions;
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

    List<decimal> _oldvalueList = new List<decimal>();

    [ObservableProperty]
    private string? _unit;

    [ObservableProperty]
    private bool? _favorit;

    [ObservableProperty]
    private string? _title;

    [ObservableProperty]
    private SolidColorBrush? _colorBrush;

    [ObservableProperty]
    private BoxShadows _colorTrendBrush;

    [ObservableProperty]
    private Color _colorTrend;

    [ObservableProperty]
    private bool? _trend;

    public DataViewModel()
    {
        ColorTrendBrush = new BoxShadows(BoxShadow.Parse("7 7 3 0 #3d3d3d"));
    }

    partial void OnValueChanged(decimal? value)
    {
        if (value != null)
        {
            if (_oldvalue != null && _oldvalueList != null)
            {
                decimal oldvalues = 0;
                _oldvalueList.ForEach(x => oldvalues += x);
                oldvalues = oldvalues / _oldvalueList.Count;
                if (value < oldvalues)
                {
                    ColorTrend = Color.Parse("#479ef5");
                    ColorTrendBrush = new BoxShadows(BoxShadow.Parse($@"7 7 3 0 #479ef5"));
                    Trend = false;
                }
                else if (value > oldvalues)
                {
                    ColorTrend = Color.Parse("#bc2f32");
                    ColorTrendBrush = new BoxShadows(BoxShadow.Parse($@"7 7 3 0 #bc2f32"));
                    Trend = true;
                }
            }
            _oldvalue = value;
            if (_oldvalue != null)
            {
                if (_oldvalueList?.Count > 5)
                    _oldvalueList.RemoveAt(0);
                _oldvalueList?.Add((decimal)_oldvalue);
            }
        }
        ColorBrush = TemperatureChanged(value);
    }

    protected decimal HotThreashold = 60;
    protected decimal ColdThreashold = 54;
    protected SolidColorBrush TemperatureChanged(decimal? value)
    {
        if (value != null)
        {
            if (value > HotThreashold)
                return new SolidColorBrush(Color.Parse("#751d1f"));
            if (value < ColdThreashold)
                return new SolidColorBrush(Color.Parse("#002c4e"));

            return new SolidColorBrush(Color.Parse("#3a1136"));
        }
        return new SolidColorBrush(Color.Parse("#292929"));
    }
}