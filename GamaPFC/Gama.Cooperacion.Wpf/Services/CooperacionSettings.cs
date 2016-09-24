using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.Services
{
    public class CooperacionSettings : ICooperacionSettings
    {
        public CooperacionSettings()
        {
            DashboardActividadesAMostrar = 15;
            DashboardCooperantesAMostrar = 25;
            ListadoDeActividadesItemsPerPage = 5;
        }

        public int DashboardCooperantesAMostrar { get; set; }
        public int DashboardActividadesAMostrar { get; set; }
        public int ListadoDeActividadesItemsPerPage { get; set; }
    }
}
