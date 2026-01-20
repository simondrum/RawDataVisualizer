using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace avaloniaCrossPlat.ViewModels.HeatIndicators
{
    public partial class OutViewModel : DataViewModel
    {
        public OutViewModel()
        {
            Name = "Out";
            Title = "Extérieur";
            HotThreashold = 30;
            ColdThreashold = 10;
        }
    }
}