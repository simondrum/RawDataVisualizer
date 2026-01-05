using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace avaloniaCrossPlat.ViewModels.HeatIndicators
{
    public partial class InOutViewModel : DataViewModel
    {
        [ObservableProperty]
        private decimal? _inTemperature;
        [ObservableProperty]
        private decimal? _outTemperature;
        [ObservableProperty]
        private decimal _instructionTemperature;

        public InOutViewModel()
        {
            Name="InOut";
            Title="Températures";
        }
    }
}