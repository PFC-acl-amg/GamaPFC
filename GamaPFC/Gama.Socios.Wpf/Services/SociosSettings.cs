using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Socios.Wpf.Services
{
    public class SociosSettings : ISociosSettings
    {
        public SociosSettings()
        {
            DashboardMesesAMostrarDeSociosNuevas = 6;
        }

        public int DashboardMesesAMostrarDeSociosNuevas { get; set; }
        
    }
}
