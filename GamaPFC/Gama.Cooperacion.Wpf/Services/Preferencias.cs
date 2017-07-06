using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.Services
{
    [Serializable]
    public class Preferencias
    {
        public static string PreferenciasPath => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                + @"\GamaData\Cooperacion\preferencias_de_cooperacion.cfg";
        public static string PreferenciasPathFolder => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                + @"\GamaData\Cooperacion\";

        public Preferencias()
        {
            DashboardUltimasPersonas = 15;
            DashboardLongitudDeNombres = 60;
            DashboardUltimasCitas = 10;
            DashboardLongitudDeSeguimientos = 60;
            DashboardUltimasAtenciones = 5;
            DashboardMesesAMostrarDeAtencionesNuevas = 6;
            DashboardMesesAMostrarDePersonasNuevas = 6;

            ListadoDePersonasItemsPerPage = 10;
            Dashboard_MostrarFiltroDeFechaPorDefecto = true;
            CitasContent_MostrarFiltroDeFechaPorDefecto = true;

            AutomaticBackupPath = PreferenciasPathFolder + @"\Backup\";

            if (!Directory.Exists(AutomaticBackupPath))
                Directory.CreateDirectory(AutomaticBackupPath);

            DoBackupOnClose = true;

            General_EdicionHabilitadaPorDefecto = false;
        }

        public string AutomaticBackupPath { get; set; }
        public bool DoBackupOnClose { get; set; }
        public DateTime? BackupDeleteDateLimit { get; set; }

        public bool General_EdicionHabilitadaPorDefecto { get; set; }

        public bool Dashboard_MostrarFiltroDeFechaPorDefecto { get; set; }
        public bool CitasContent_MostrarFiltroDeFechaPorDefecto { get; set; }

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
            new BinaryFormatter().Serialize(File.Create(PreferenciasPath), this);
        }
    }
}
