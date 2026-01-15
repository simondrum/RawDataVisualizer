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
    private double _refreshProgress = 100; // 0 → 100
    private readonly TimeSpan _refreshInterval = TimeSpan.FromSeconds(30);
    private readonly TimeSpan _uiTick = TimeSpan.FromMilliseconds(100);
    private DateTime _lastRefresh;

    public MainViewModel()
    {
        Code = "28559";
        StartGlobalTimer();
        StartAnimationTimer();
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
    private DataViewModel? _inViewModel;
    [ObservableProperty]
    private DataViewModel? _outViewModel;
    [ObservableProperty]
    private DataViewModel? _instructionViewModel;
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

            foreach (var element in array)
            {
                string? name = null;
                try
                {
                    name = element.GetProperty("name").GetString() ?? "";
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error => {ex.Message} \n {element}");
                }
                if (String.IsNullOrEmpty(name))
                    continue;
                string? nullValue = element.GetProperty("value").GetString();
                if (nullValue is null)
                    continue;

                string value = nullValue;


                ViewModelBase? data = GetViewModel(name);
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
                    if (Decimal.TryParse(element.GetProperty("value").GetString(), CultureInfo.InvariantCulture, out var valueDouble))
                        dataViewModel.Value = valueDouble;
                    dataViewModel.Unit = element.GetProperty("unit").GetString();
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

    private ViewModelBase? GetViewModel(string name)
    {
        switch (name)
        {
            // case "M1 R1 POMPE CHAUFFAGE MUR CHAUFFANT ":
            //     return "Chauffage allumé";
            case "1 SONDE CAPTEUR":
                {
                    if (SolarPanelViewModel == null)
                        SolarPanelViewModel = new SolarPanelViewModel();
                    return SolarPanelViewModel;
                }
            case "5 SONDE HAUT BALLON ECS":
                {
                    if (UpperHotWaterViewModel == null)
                        UpperHotWaterViewModel = new UpperHotWaterViewModel();
                    return UpperHotWaterViewModel;
                }
            case "8 SONDE POELE":
                {
                    if (StoveViewModel == null)
                        StoveViewModel = new StoveViewModel();
                    return StoveViewModel;
                }
            // case "R9 POMPE BOUILLEUR":
            //     return "Poele chauffe eau";
            case "12 SONDE EXTERIEUR":
                {
                    if (InViewModel == null)
                        InViewModel = new DataViewModel();
                    return InViewModel;
                }
            case "M1 S2 SONDE AMBIANCE MUR CHAUFFANT ":
                {
                    if (OutViewModel == null)
                        OutViewModel = new DataViewModel();
                    return OutViewModel;
                }
            case "M1 S3 SONDE DE COMPENSATION INTERRUPTEUR CHAUFFAGE":
                {
                    if (InstructionViewModel == null)
                        InstructionViewModel = new DataViewModel();
                    return InstructionViewModel;
                }
            // case "3 SONDE ENTREE CHAUDE ECHANGEUR SOLAIRE ":
            //     return new DataViewModel();
            // case "Sortie B PWM SECONDAIRE":
            //     return new DataViewModel();
            case "7 SONDE BALLON DÉPART ":
                {
                    if (MidHotWaterViewModel == null)
                        MidHotWaterViewModel = new MidHotWaterViewModel();
                    return MidHotWaterViewModel;
                }
            case "2 SONDE BAS BALLON ":
                {
                    if (LowHotWaterViewModel == null)
                        LowHotWaterViewModel = new LowHotWaterViewModel();
                    return LowHotWaterViewModel;
                }
            default:
                return null; // new DataViewModel(); ;
        }

    }
}

