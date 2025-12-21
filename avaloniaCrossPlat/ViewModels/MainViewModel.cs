using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
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
    private ObservableCollection<DataViewModel> _data = new ObservableCollection<DataViewModel>();
    [ObservableProperty]
    private ObservableCollection<DataViewModel> _dataTemp = new ObservableCollection<DataViewModel>();
    [ObservableProperty]
    private ObservableCollection<DataViewModel> _dataPourcent = new ObservableCollection<DataViewModel>();


    public MainViewModel()
    {
    }

    partial void OnEndPointChanged(string? oldValue, string? newValue)
    {
        if (String.IsNullOrEmpty(newValue))
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

            var service = new HttpService();

            var json = await service.GetInfos(EndPoint);
            var jsonArray = JsonSerializer.Deserialize<JsonArray>(json);
            if (jsonArray is null)
                return;
            foreach (var element in jsonArray)
            {
                if (element is not JsonObject obj)
                    continue;

                Data.Add(new DataViewModel
                {
                    Id = obj["id"]?.GetValue<string>(),
                    Name = obj["name"]?.GetValue<string>(),
                    Value = obj["value"]?.GetValue<string>(),
                    Unit = obj["unit"]?.GetValue<string>(),
                    Favorit = IsFavorite(obj["name"]?.GetValue<string>())
                });

            }
            string tempUnit = "";
            foreach (var data in Data.OrderBy(x => x.Unit))
            {
                if (tempUnit != data.Unit)

                {
                    tempUnit = data.Unit;
                    Console.WriteLine($"===== {tempUnit}");

                }
                Console.WriteLine($"{data.Name}");
            }
            DataTemp = new ObservableCollection<DataViewModel>(Data.Where(x => x.Unit == " °C").OrderBy(x => x.Favorit));
            DataPourcent = new ObservableCollection<DataViewModel>(Data.Where(x => x.Unit == " %").OrderBy(x => x.Favorit));
            if (Data != null && Data.Count > 0)
            {
                Dispatcher.UIThread.Invoke(() => { IsDataValid = true; });
            }
        }
        catch (Exception)
        {
            Dispatcher.UIThread.Invoke(() => { IsDataValid = false; });
        }
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
