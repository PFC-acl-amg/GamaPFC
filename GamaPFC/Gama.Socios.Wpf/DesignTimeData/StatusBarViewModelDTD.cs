using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Socios.Wpf.DesignTimeData
{
    public class StatusBarViewModelDTD : ViewModelBase
    {
        private string _Mensaje;

        public StatusBarViewModelDTD()
        {
            Mensaje = "Sin mensaje...";
        }

        public string Mensaje
        {
            get { return _Mensaje; }
            set { SetProperty(ref _Mensaje, value); }
        }
    }
}
