using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace avaloniaCrossPlat.ViewModels.HeatIndicators
{
    public class LowHotWaterViewModel : DataViewModel
    {
        
        public LowHotWaterViewModel()
        {
            HotThreashold = 50;
            ColdThreashold = 40;
        }
    }
}