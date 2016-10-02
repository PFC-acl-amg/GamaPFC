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
        }

        public int DashboardLongitudDeNombres { get; set; }
        public int DashboardUltimasPersonas { get; set; }
    }
}
