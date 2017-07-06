using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Socios.Wpf.Services
{
    [Serializable]
    public class Preferencias
    {
        public static string PreferenciasPath => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                + @"\GamaData\Socios\preferencias_de_socios.cfg";
        public static string PreferenciasPathFolder => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                + @"\GamaData\Socios\";

        public Preferencias()
        {
            ListadoDeSociosItemsPerPage = 10;
            MesesParaSerConsideradoMoroso = 3;

            AutomaticBackupPath = PreferenciasPathFolder + @"\Backup\";
            DoBackupOnClose = true;
            General_EdicionHabilitadaPorDefecto = false;

            if (!Directory.Exists(AutomaticBackupPath))
                Directory.CreateDirectory(AutomaticBackupPath);
        }

        public string AutomaticBackupPath { get; set; }
        public bool DoBackupOnClose { get; set; }
        public DateTime? BackupDeleteDateLimit { get; set; }

        public bool General_EdicionHabilitadaPorDefecto { get; set; }
        public int MesesParaSerConsideradoMoroso { get; set; }
        public int ListadoDeSociosItemsPerPage { get; set; }
        public float CuotaMensualPredeterminada { get; set; }

        public void Serializar()
        {
            new BinaryFormatter().Serialize(File.Create(PreferenciasPath), this);
        }
    }
}