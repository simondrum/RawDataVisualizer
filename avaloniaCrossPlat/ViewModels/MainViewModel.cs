using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Threading;
using avaloniaCrossPlat.Services;
using avaloniaCrossPlat.ViewModels.HeatIndicators;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace avaloniaCrossPlat.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool? _isDataValid = null;

    [ObservableProperty]
    private bool? _error = false;

    [ObservableProperty]
    private string? _endPoint = "";

    [ObservableProperty]
    private string? _code = null;

    [ObservableProperty]
    private string? _pompeSolaire = null;

[ObservableProperty]
    private string? _pompePoele = null;

    [ObservableProperty]
    private string? _pompeMur = null;

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
    private IServiceProvider Services;

    public MainViewModel()
    {
        Code = "28559";
        return;
        if (Application.Current is App app)
        {
            Services = app.Services;
            if (Services != null)
            {
                Console.WriteLine("Services is not null");
                try
                {
                    var serviceStorage = Services.GetRequiredService<IClientContextStorage>();
                    if (serviceStorage != null)
                    {
                        Console.WriteLine("serviceStorage is not null");
                        var clientId = serviceStorage.GetClientId();

                        if (!String.IsNullOrEmpty(clientId))
                        {
                            Code = clientId;
                        }
                        Console.WriteLine($"clientId => {clientId}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            //     Console.WriteLine($"ClientId => {Services.GetRequiredService<IClientContextStorage>()?.GetClientId()}");
        }
    }

    partial void OnCodeChanged(string? value)
    {
        if (!String.IsNullOrEmpty(value) && value.Length == 5)
        {
            Send();
        }
    }

    partial void OnErrorChanged(bool? value)
    {
        if (value != null && (bool)value)
        {
            var timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(5)
            };
            timer.Tick += async (_, __) =>
            {
                timer.Stop();
                Dispatcher.UIThread.Invoke(() =>
                {
                    Error = false;
                });
            };
            timer.Start();
        }
    }

    public void Send()
    {
        if (String.IsNullOrEmpty(Code))
            IsDataValid = false;
        else
        {
            LoadData();
        }
    }

    #region data indicators
    [ObservableProperty]
    private InOutViewModel? _inOutViewModel;
    [ObservableProperty]
    private LowHotWaterViewModel? _lowHotWaterViewModel;
    [ObservableProperty]
    private MidHotWaterViewModel? _midHotWaterViewModel;
    [ObservableProperty]
    private SolarPanelViewModel? _solarPanelViewModel;
    [ObservableProperty]
    private StoveViewModel? _stoveViewModel;
    [ObservableProperty]
    private UpperHotWaterViewModel? _upperHotWaterViewModel;
    #endregion

    private async void LoadData()
    {
        try
        {
            if (Code == null)
                return;
            var service = new HttpService();

            var json = await service.GetInfos("28559");
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            var array = root.EnumerateArray();

            if (array.Count() == 0)
                throw new ArgumentNullException();

            List<DataViewModel> tempData = new List<DataViewModel>(); ;

            InOutViewModel = new InOutViewModel();

            foreach (var element in array)
            {
                try
                {
                    string? name = element.GetProperty("name").GetString() ?? "";
                    if (name is null)
                        continue;
                    ViewModelBase data = GetViewModel(name);
                    if (data is null)
                        continue;

                    string? nullValue = element.GetProperty("value").GetString();
                    if (nullValue is null)
                        continue;

                    string value = (string)nullValue;

                    if (data is InOutViewModel)
                    {
                        switch (name)
                        {
                            case "12 SONDE EXTERIEUR":
                                InOutViewModel.OutTemperature = Decimal.Parse(value, CultureInfo.InvariantCulture);
                                break;
                            case "M1 S2 SONDE AMBIANCE MUR CHAUFFANT ":
                                InOutViewModel.InTemperature = Decimal.Parse(value, CultureInfo.InvariantCulture);
                                break;
                            case "M1 S3 SONDE DE COMPENSATION INTERRUPTEUR CHAUFFAGE":
                                InOutViewModel.InstructionTemperature = Decimal.Parse(value, CultureInfo.InvariantCulture);
                                break;
                        }
                        continue;
                    }
                    if (data is UpperHotWaterViewModel upperHotWaterViewModel)
                    {
                        upperHotWaterViewModel.Value = element.GetProperty("value").GetString();
                        continue;
                    }
                    if (data is MidHotWaterViewModel midHotWaterViewModel)
                    {
                        midHotWaterViewModel.Value = element.GetProperty("value").GetString();
                        continue;
                    }
                    if (data is LowHotWaterViewModel lowHotWaterViewModel)
                    {
                        lowHotWaterViewModel.Value = element.GetProperty("value").GetString();
                        continue;
                    }
                    if (data is StoveViewModel stoveViewModel)
                    {
                        stoveViewModel.Value = element.GetProperty("value").GetString();
                        continue;
                    }
                    if (data is SolarPanelViewModel solarPanelViewModel)
                    {
                        solarPanelViewModel.Value = element.GetProperty("value").GetString();
                        continue;
                    }
                    if (name == "M1 R1 POMPE CHAUFFAGE MUR CHAUFFANT ")
                    {
                        PompeMur = element.GetProperty("value").GetString();
                        continue;
                    }
                    if (name == "R5 POMPE SOLAIRE PRIMAIRE ")
                    {
                        PompeSolaire = element.GetProperty("value").GetString();
                        continue;
                    }
                    if (name == "R9 POMPE BOUILLEUR")
                    {
                        PompePoele = element.GetProperty("value").GetString();
                        continue;
                    }

                    if (data is DataViewModel dataViewModel)
                    {
                        dataViewModel.Id = element.GetProperty("id").GetString();
                        dataViewModel.Name = element.GetProperty("name").GetString();
                        dataViewModel.Value = element.GetProperty("value").GetString();
                        dataViewModel.Unit = element.GetProperty("unit").GetString();
                        if (data != null)
                        {

                            if (!String.IsNullOrEmpty(dataViewModel.Name))
                            {
                                dataViewModel.Favorit = IsFavorite(dataViewModel.Name);
                                dataViewModel.Title = GetLibelle(dataViewModel.Name);
                            }
                            tempData.Add(dataViewModel);
                        }
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
                    Error = false;
                    try
                    {
                        Services.GetRequiredService<IClientContextStorage>()?.SetClientId(Code);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                    StartGlobalTimer();
                    StartAnimationTimer();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception : {ex}");
            Code = "";
            Error = true;
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
            case "M1 R1 POMPE CHAUFFAGE MUR CHAUFFANT ": //La maison chauffe
            case "1 SONDE CAPTEUR": //capteur solaire
            case "5 SONDE HAUT BALLON ECS": //Eau chaude chauffage
            case "8 SONDE POELE": //température poele
            case "R9 POMPE BOUILLEUR": //Poele chauffe eau
            case "12 SONDE EXTERIEUR": //température extérieure 
            case "M1 S2 SONDE AMBIANCE MUR CHAUFFANT ": //température intérieure
            case "M1 S3 SONDE DE COMPENSATION INTERRUPTEUR CHAUFFAGE": //consigne            
            case "3 SONDE ENTREE CHAUDE ECHANGEUR SOLAIRE ": //température solaire vers maison
            case "Sortie B PWM SECONDAIRE": //Pourcentage solaire vers eau chaude
            case "7 SONDE BALLON DEPART ": // réserve chauffage
            case "2 SONDE BAS BALLON ":
                return true;
            default:
                return false;
        }

    }

    private string? GetLibelle(string name)
    {
        switch (name)
        {
            case "M1 R1 POMPE CHAUFFAGE MUR CHAUFFANT ":
                return "Chauffage allumé";
            case "1 SONDE CAPTEUR":
                return "capteur solaire";
            case "5 SONDE HAUT BALLON ECS":
                return "Eau chaude chauffage";
            case "8 SONDE POELE":
                return "température poele";
            case "R9 POMPE BOUILLEUR":
                return "Poele chauffe eau";
            case "12 SONDE EXTERIEUR":
                return "température extérieure";
            case "M1 S2 SONDE AMBIANCE MUR CHAUFFANT ":
                return "température intérieure";
            case "M1 S3 SONDE DE COMPENSATION INTERRUPTEUR CHAUFFAGE":
                return "consigne";
            case "3 SONDE ENTREE CHAUDE ECHANGEUR SOLAIRE ":
                return "température solaire vers maison";
            case "Sortie B PWM SECONDAIRE":
                return "Pourcentage solaire vers eau chaude";
            case "7 SONDE BALLON DEPART ":
                return "réserve chauffage";
            case "2 SONDE BAS BALLON ":
                return "Bas du ballon";
            default:
                return name;
        }

    }

    private ViewModelBase? GetViewModel(string? name)
    {
        switch (name)
        {
            // case "M1 R1 POMPE CHAUFFAGE MUR CHAUFFANT ":
            //     return "Chauffage allumé";
            case "1 SONDE CAPTEUR":
                {
                    SolarPanelViewModel = new SolarPanelViewModel();
                    return SolarPanelViewModel;
                }
            case "5 SONDE HAUT BALLON ECS":
                {
                    UpperHotWaterViewModel = new UpperHotWaterViewModel();
                    return UpperHotWaterViewModel;
                }
            case "8 SONDE POELE":
                {
                    StoveViewModel = new StoveViewModel();
                    return StoveViewModel;
                }
            // case "R9 POMPE BOUILLEUR":
            //     return "Poele chauffe eau";
            case "12 SONDE EXTERIEUR":
                return InOutViewModel;
            case "M1 S2 SONDE AMBIANCE MUR CHAUFFANT ":
                return InOutViewModel;
            case "M1 S3 SONDE DE COMPENSATION INTERRUPTEUR CHAUFFAGE":
                return InOutViewModel;
            // case "3 SONDE ENTREE CHAUDE ECHANGEUR SOLAIRE ":
            //     return new DataViewModel();
            // case "Sortie B PWM SECONDAIRE":
            //     return new DataViewModel();
            case "7 SONDE BALLON DÉPART ":
                {
                    MidHotWaterViewModel = new MidHotWaterViewModel();
                    return MidHotWaterViewModel;
                }
            case "2 SONDE BAS BALLON ":
                {
                    LowHotWaterViewModel = new LowHotWaterViewModel();
                    return LowHotWaterViewModel;
                }
            default:
                return new DataViewModel(); ;
        }

    }
}

