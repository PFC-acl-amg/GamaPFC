using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Atenciones.Wpf.Services
{
    [Serializable]
    public class PreferenciasDeAtenciones
    {
        public PreferenciasDeAtenciones()
        {
            DashboardUltimasPersonas = 15;
            DashboardLongitudDeNombres = 60;
            DashboardUltimasCitas = 10;
            DashboardLongitudDeSeguimientos = 60;
            DashboardUltimasAtenciones = 5;
            DashboardMesesAMostrarDeAtencionesNuevas = 6;
            DashboardMesesAMostrarDePersonasNuevas = 6;

            ListadoDePersonasItemsPerPage = 10;
        }

        public int DashboardLongitudDeNombres { get; set; }
        public int DashboardUltimasPersonas { get; set; }
        public int DashboardUltimasCitas { get; set; }
        public int DashboardLongitudDeSeguimientos { get; set; }
        public int DashboardUltimasAtenciones { get; set; }
        public int DashboardMesesAMostrarDeAtencionesNuevas { get; set; }
        public int DashboardMesesAMostrarDePersonasNuevas { get; set; }
        public int ListadoDePersonasItemsPerPage { get; set; }

        public void Serializar()
        {
            string preferenciasPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                + @"\preferencias_de_atenciones.cfg";

            new BinaryFormatter().Serialize(File.Create(preferenciasPath), this);
        }
    }
}
