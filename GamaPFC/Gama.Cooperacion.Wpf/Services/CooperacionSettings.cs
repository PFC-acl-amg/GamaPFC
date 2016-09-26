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
            DashboardActividadesLongitudDeTitulos = 40;
            DashboardActividadesAMostrar = 15;
            DashboardCooperantesAMostrar = 25;
            DashboardMesesAMostrarDeActividadesNuevas = 6;
            DashboardMesesAMostrarDeCooperantesNuevos = 6;
            ListadoDeActividadesItemsPerPage = 10;
        }

        public int DashboardCooperantesAMostrar { get; set; }
        public int DashboardActividadesAMostrar { get; set; }
        public int DashboardMesesAMostrarDeActividadesNuevas { get; set; }
        public int ListadoDeActividadesItemsPerPage { get; set; }
        public int DashboardActividadesLongitudDeTitulos { get; set; }
        public int DashboardMesesAMostrarDeCooperantesNuevos { get; set; }
    }
}
