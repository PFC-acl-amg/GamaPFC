using Core;
using Gama.Socios.Wpf.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Socios.Wpf.Wrappers
{
    public class PreferenciasDeSociosWrapper : ModelWrapper<PreferenciasDeSocios>
    {
        public PreferenciasDeSociosWrapper(PreferenciasDeSocios model) : base(model)
        {

        }

        ///
        /// Dashboard
        ///
       
        public int DashboardMesesAMostrarDeSociosNuevos
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }
        public int DashboardMesesAMostrarDeSociosNuevosOriginalValue 
            => GetOriginalValue<int>(nameof(DashboardMesesAMostrarDeSociosNuevos));
        public bool DashboardMesesAMostrarDeSociosNuevosIsChanged
            => GetIsChanged(nameof(DashboardMesesAMostrarDeSociosNuevos));

        public int DashboardUltimosSocios
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }
        public int DashboardUltimosSociosOriginalValue 
            => GetOriginalValue<int>(nameof(DashboardUltimosSocios));
        public bool DashboardUltimosSociosIsChanged 
            => GetIsChanged(nameof(DashboardUltimosSocios));

        public int DashboardSociosCumpliendoBirthdays
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }
        public int DashboardSociosCumpliendoBirthdaysOriginalValue
            => GetOriginalValue<int>(nameof(DashboardSociosCumpliendoBirthdays));
        public bool DashboardSociosCumpliendoBirthdaysIsChanged
            => GetIsChanged(nameof(DashboardSociosCumpliendoBirthdays));

        public int DashboardSociosMorosos
        { 
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }
        public int DashboardSociosMorososOriginalValue 
            => GetOriginalValue<int>(nameof(DashboardSociosMorosos));
        public bool DashboardSociosMorososIsChanged 
            => GetIsChanged(nameof(DashboardSociosMorosos));

        ///
        /// Socios
        ///

        public int ListadoDeSociosItemsPerPage
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }
        public int ListadoDeSociosItemsPerPageOriginalValue => GetOriginalValue<int>(nameof(ListadoDeSociosItemsPerPage));
        public bool ListadoDeSociosItemsPerPageIsChanged => GetIsChanged(nameof(ListadoDeSociosItemsPerPage));

        ///
        /// Contabilidad
        ///
    
        public int MesesParaSerConsideradoMoroso
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }
        public int MesesParaSerConsideradoMorosoOriginalValue => GetOriginalValue<int>(nameof(MesesParaSerConsideradoMoroso));
        public bool MesesParaSerConsideradoMorosoIsChanged => GetIsChanged(nameof(MesesParaSerConsideradoMoroso));

        public float CuotaMensualPredeterminada
        {
            get { return GetValue<float>(); }
            set { SetValue(value); }
        }
        public float CuotaMensualPredeterminadaOriginalValue => GetOriginalValue<float>(nameof(CuotaMensualPredeterminada));
        public bool CuotaMensualPredeterminadaIsChanged => GetIsChanged(nameof(CuotaMensualPredeterminada));
    }
}
