using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
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
    private string? _value;

    [ObservableProperty]
    private string? _unit;

    [ObservableProperty]
    private bool? _favorit;

    [ObservableProperty]
    private string? _title;
}