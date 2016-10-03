using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Atenciones.Wpf.Services
{
    public class AtencionesSettings : IAtencionesSettings
    {
        public AtencionesSettings()
        {
            DashboardUltimasPersonas = 15;
            DashboardLongitudDeNombres = 60;
            DashboardUltimasCitas = 10;
            DashboardLongitudDeSeguimientos = 60;
            DashboardUltimasAtenciones = 5;
            DashboardMesesAMostrarDeAtencionesNuevas = 6;
            DashboardMesesAMostrarDePersonasNuevas = 6;
        }

        public int DashboardLongitudDeNombres { get; set; }
        public int DashboardUltimasPersonas { get; set; }
        public int DashboardUltimasCitas { get; set; }
        public int DashboardLongitudDeSeguimientos { get; set; }
        public int DashboardUltimasAtenciones { get; set; }
        public int DashboardMesesAMostrarDeAtencionesNuevas { get; set; }
        public int DashboardMesesAMostrarDePersonasNuevas { get; set; }
    }
}
