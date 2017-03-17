using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Socios.Wpf.Services
{
    public interface IPreferenciasDeSocios
    {
        int DashboardMesesAMostrarDeSociosNuevos { get; set; }
        int DashboardUltimosSocios { get; set; }
        int ListadoDeSociosItemsPerPage { get; set; }
        int MesesParaSerConsideradoMoroso { get; set; }
        float CuotaMensualPredeterminada { get; set; }
    }
}
