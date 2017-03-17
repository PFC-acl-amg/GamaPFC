using Core;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Socios.Wpf.Services
{
    public class PreferenciasDeSocios : BindableBase
    {
        public PreferenciasDeSocios()
        {
            DashboardMesesAMostrarDeSociosNuevos = 6;
            DashboardUltimosSocios = 15;
            DashboardSociosCumpliendoBirthdays = 15;
            DashboardSociosMorosos = 5;
            ListadoDeSociosItemsPerPage = 10;
            MesesParaSerConsideradoMoroso = 3;
            CuotaMensualPredeterminada = 10;
        }

        private int _DashboardMesesAMostrarDeSociosNuevos;
        public int DashboardMesesAMostrarDeSociosNuevos
        {
            get { return _DashboardMesesAMostrarDeSociosNuevos; }
            set { SetProperty(ref _DashboardMesesAMostrarDeSociosNuevos, value); }
        }
        
        private int _DashboardUltimosSocios;
        public int DashboardUltimosSocios
        {
            get { return _DashboardUltimosSocios; }
            set { SetProperty(ref _DashboardUltimosSocios, value); }
        }

        private int _DashboardSociosCumpliendoBirthdays;
        public int DashboardSociosCumpliendoBirthdays
        {
            get { return _DashboardSociosCumpliendoBirthdays; }
            set { SetProperty(ref _DashboardSociosCumpliendoBirthdays, value); }
        }
        
        public int DashboardSociosMorosos { get; set; }

        public int ListadoDeSociosItemsPerPage { get; set; }
        public int MesesParaSerConsideradoMoroso { get; set; }
        public float CuotaMensualPredeterminada { get; set; }
    }
}
