using Core;
using Gama.Cooperacion.Business;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.DesignTimeData
{
    public class DesignTimeDataViewModel : ViewModelBase
    {
        public ObservableCollection<Cooperante> Cooperantes { get; private set; }

        public DesignTimeDataViewModel()
        {
            Cooperantes = new ObservableCollection<Cooperante>();

            Cooperantes.Add(new Cooperante());
        }
    }
}
