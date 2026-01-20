using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace avaloniaCrossPlat.ViewModels.HeatIndicators
{
    public partial class InstructionTemperatureViewModel : DataViewModel
    {
        public InstructionTemperatureViewModel()
        {
            Name = "Instruction";
            Title = "Consigne";
            HotThreashold = 1;
            ColdThreashold = -1;
        }
    }
}