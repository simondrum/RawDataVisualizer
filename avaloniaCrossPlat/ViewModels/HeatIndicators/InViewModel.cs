using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace avaloniaCrossPlat.ViewModels.HeatIndicators
{
    public partial class InViewModel : DataViewModel
    {
        public InViewModel()
        {
            Name = "Interior";
            Title = "Intérieur";
            HotThreashold = 21;
            ColdThreashold = 19;
        }
    }
}