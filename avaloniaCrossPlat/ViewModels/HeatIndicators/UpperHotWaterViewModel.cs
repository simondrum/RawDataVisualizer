using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Media;

namespace avaloniaCrossPlat.ViewModels.HeatIndicators
{
    public class UpperHotWaterViewModel : DataViewModel
    {
        public UpperHotWaterViewModel()
        {
            HotThreashold = 60;
            ColdThreashold = 54;
        }
    }
}