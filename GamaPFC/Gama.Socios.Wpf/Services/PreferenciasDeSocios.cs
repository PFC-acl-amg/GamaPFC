using Core;
using Prism.Mvvm;
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
    public class PreferenciasDeSocios
    {
        public PreferenciasDeSocios()
        {
            DashboardMesesAMostrarDeSociosNuevos = 6;
            DashboardUltimosSocios = 15;
            DashboardSociosCumpliendoBirthdays = 15;
            DashboardSociosMorosos = 15;

            ListadoDeSociosItemsPerPage = 10;

            MesesParaSerConsideradoMoroso = 3;
            CuotaMensualPredeterminada = 10;
        }
        
        public int DashboardMesesAMostrarDeSociosNuevos { get; set; }
        public int DashboardUltimosSocios { get; set; }
        public int DashboardSociosCumpliendoBirthdays { get; set; }
        public int DashboardSociosMorosos { get; set; }

        public int ListadoDeSociosItemsPerPage { get; set; }

        public int MesesParaSerConsideradoMoroso { get; set; }
        public float CuotaMensualPredeterminada { get; set; }

        public void Serializar()
        {
            string preferenciasPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\preferencias_de_socios.cfg";

            new BinaryFormatter().Serialize(File.Create(preferenciasPath), this);
        }
    }
}
