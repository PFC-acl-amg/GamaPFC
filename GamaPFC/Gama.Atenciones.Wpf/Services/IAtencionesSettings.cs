using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Atenciones.Wpf.Services
{
    public interface IAtencionesSettings
    {
        int DashboardLongitudDeSeguimientos { get; set; }
        int DashboardMesesAMostrarDeAtencionesNuevas { get; set; }
        int DashboardMesesAMostrarDePersonasNuevas { get; set; }
        int DashboardUltimasAtenciones { get; set; }
        int DashboardUltimasCitas { get; set; }
        int DashboardUltimasPersonas { get; set; }
    }
}
