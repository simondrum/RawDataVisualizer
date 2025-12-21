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
    private string? _code = null;

    [ObservableProperty]
    private ObservableCollection<DataViewModel> _data = new ObservableCollection<DataViewModel>();
    [ObservableProperty]
    private ObservableCollection<DataViewModel> _dataTemp = new ObservableCollection<DataViewModel>();
    [ObservableProperty]
    private ObservableCollection<DataViewModel> _dataPourcent = new ObservableCollection<DataViewModel>();


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
                    Data.Add(data);
                }
                }catch(Exception)
                {
                    
                }

            }
            Console.WriteLine("3");
            string tempUnit = "";
            foreach (var data in Data.OrderBy(x => x.Unit))
            {
                if (tempUnit != data.Unit && data.Unit != null)

                {
                    tempUnit = data.Unit;
                    Console.WriteLine($"===== {tempUnit}");

                }
                Console.WriteLine($"{data.Name}");
            }
            Console.WriteLine("6");
            DataTemp = new ObservableCollection<DataViewModel>(Data.Where(x => x.Unit == " °C").OrderBy(x => x.Favorit));
            DataPourcent = new ObservableCollection<DataViewModel>(Data.Where(x => x.Unit == " %").OrderBy(x => x.Favorit));
            Console.WriteLine("7");
            if (Data != null && Data.Count > 0)
            {
                IsDataValid = true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception : {ex}");

            IsDataValid = false;
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
