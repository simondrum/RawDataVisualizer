using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace avaloniaCrossPlat.ViewModels.HeatIndicators
{
    public class MidHotWaterViewModel : DataViewModel
    {
        public MidHotWaterViewModel()
        {
            HotThreashold = 55;
            ColdThreashold = 45;
        }
    }
}