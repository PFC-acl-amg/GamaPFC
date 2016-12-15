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
            DashboardMesesAMostrarDeSociosNuevos = 6;
            DashboardUltimosSocios = 15;
            ListadoDeSociosItemsPerPage = 25;
        }

        public int DashboardMesesAMostrarDeSociosNuevos { get; set; }
        public int DashboardUltimosSocios { get; set; }
        public int ListadoDeSociosItemsPerPage { get; set; }
    }
}
