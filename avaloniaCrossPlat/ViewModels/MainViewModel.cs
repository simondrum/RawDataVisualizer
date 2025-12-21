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

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool? _isDataValid = false;

    [ObservableProperty]
    private string? _endPoint = "";

    [ObservableProperty]
    private string? _code = null;

    [ObservableProperty]
    private ObservableCollection<DataViewModel> _data = new ObservableCollection<DataViewModel>();
    [ObservableProperty]
    private ObservableCollection<DataViewModel> _dataTemp = new ObservableCollection<DataViewModel>();
    [ObservableProperty]
    private ObservableCollection<DataViewModel> _dataPourcent = new ObservableCollection<DataViewModel>();

    [ObservableProperty]
    private double _refreshProgress = 100; // 0 → 100
    private readonly TimeSpan _refreshInterval = TimeSpan.FromSeconds(30);
    private readonly TimeSpan _uiTick = TimeSpan.FromMilliseconds(100);
    private DateTime _lastRefresh;

    public MainViewModel()
    {
        if (!String.IsNullOrEmpty(EndPoint))
        {
            LoadData();
        }
    }

    partial void OnCodeChanged(string? value)
    {
        if (!String.IsNullOrEmpty(value) && value.Length == 5)
        {
            Send();
        }
    }

    public void Send()
    {
        if (Code == null)
            IsDataValid = false;
        else
        {
            LoadData();
        }
    }
    private async void LoadData()
    {
        try
        {
            if (Code == null)
                return;
            var service = new HttpService();

            Console.WriteLine("1");
            var json = await service.GetInfos(Code);
            Console.WriteLine("2");
            using var doc = JsonDocument.Parse(json);
            Console.WriteLine("3");

            var root = doc.RootElement;
            Console.WriteLine("4");
            var array = root.EnumerateArray();
            Console.WriteLine("5");

            List<DataViewModel> tempData = new List<DataViewModel>(); ;
            foreach (var element in array)
            {
                try
                {

                    var data = new DataViewModel
                    {
                        Id = element.GetProperty("id").GetString(),
                        Name = element.GetProperty("name").GetString(),
                        Value = element.GetProperty("value").GetString(),
                        Unit = element.GetProperty("unit").GetString()
                    };
                    if (data != null)
                    {

                        if (!String.IsNullOrEmpty(data.Name))
                            data.Favorit = IsFavorite(data.Name);
                        tempData.Add(data);
                    }
                }
                catch (Exception)
                {

                }

            }
            Console.WriteLine("3");
            string tempUnit = "";
            foreach (var data in tempData.OrderBy(x => x.Unit))
            {
                if (tempUnit != data.Unit && data.Unit != null)

                {
                    tempUnit = data.Unit;
                    Console.WriteLine($"===== {tempUnit}");

                }
                Console.WriteLine($"{data.Name}");
            }
            Console.WriteLine("6");
            Console.WriteLine("7");
            if (tempData != null && tempData.Count > 0)
            {
                DataTemp = new ObservableCollection<DataViewModel>(tempData.Where(x => x.Unit == " °C").OrderBy(x => x.Favorit));
                DataPourcent = new ObservableCollection<DataViewModel>(tempData.Where(x => x.Unit == " %").OrderBy(x => x.Favorit));
                Data = new ObservableCollection<DataViewModel>(tempData);
                if (IsDataValid == null || !(bool)IsDataValid)
                {
                    IsDataValid = true;
                    StartGlobalTimer();
                    StartAnimationTimer();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception : {ex}");

            IsDataValid = false;
        }
    }

    private void StartGlobalTimer()
    {
        var timer = new DispatcherTimer()
        {
            Interval = _refreshInterval
        };
        timer.Tick += async (_, __) =>
        {
            _lastRefresh = DateTime.UtcNow;
            RefreshProgress = 100;
            await RefreshDataAsync();

        };

        timer.Start();
        _lastRefresh = DateTime.UtcNow;
    }
    private void StartAnimationTimer()
    {
        var timer = new DispatcherTimer()
        {
            Interval = _uiTick
        };
        timer.Tick += async (_, __) =>
        {
            var elapsed = DateTime.UtcNow - _lastRefresh;
            var ratio = elapsed.TotalMilliseconds / _refreshInterval.TotalMilliseconds;

            RefreshProgress = Math.Max(0, 100 - ratio * 100);
        };

        timer.Start();
    }

    private async Task RefreshDataAsync()
    {
        LoadData();
    }

    private bool? IsFavorite(string name)
    {
        switch (name)
        {
            case "M1 R1 POMPE CHAUFFAGE MUR CHAUFFANT ":
            case "1 SONDE CAPTEUR":
            case "5 SONDE HAUT BALLON ECS":
            case "8 SONDE POELE":
            case "12 SONDE EXTERIEUR":
            case "M1 S2 SONDE AMBIANCE MUR CHAUFFANT ":
            case "2 SONDE BAS BALLON ":
                return true;
            default:
                return false;
        }

    }
}
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
}
