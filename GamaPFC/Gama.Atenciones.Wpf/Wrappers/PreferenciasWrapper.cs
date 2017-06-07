using Core;
using Gama.Atenciones.Wpf.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Atenciones.Wpf.Wrappers
{
    public class PreferenciasWrapper : ModelWrapper<Preferencias>
    {
        public PreferenciasWrapper(Preferencias model) : base(model)
        {

        }

        ///
        /// Copias de Seguridad
        ///
        public string AutomaticBackupPath
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string AutomaticBackupPathOriginalValue => GetOriginalValue<string>(nameof(AutomaticBackupPath));
        public bool AutomaticBackupPathIsChanged => GetIsChanged(nameof(AutomaticBackupPath));

        ///
        /// Dashboard
        ///
        public bool Dashboard_MostrarFiltroDeFechaPorDefecto
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }
        public bool Dashboard_MostrarFiltroDeFechaPorDefectoOriginalValue
            => GetOriginalValue<bool>(nameof(Dashboard_MostrarFiltroDeFechaPorDefecto));
        public bool Dashboard_MostrarFiltroDeFechaPorDefectoIsChanged
            => GetIsChanged(nameof(Dashboard_MostrarFiltroDeFechaPorDefecto));

        public int DashboardUltimasPersonas
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }
        public int DashboardUltimasPersonasOriginalValue
            => GetOriginalValue<int>(nameof(DashboardUltimasPersonas));
        public bool DashboardUltimasPersonasIsChanged
            => GetIsChanged(nameof(DashboardUltimasPersonas));

        public int DashboardUltimasAtenciones
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }
        public int DashboardUltimasAtencionesOriginalValue
            => GetOriginalValue<int>(nameof(DashboardUltimasAtenciones));
        public bool DashboardUltimasAtencionesIsChanged
            => GetIsChanged(nameof(DashboardUltimasAtenciones));

        public int DashboardUltimasCitas
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }
        public int DashboardUltimasCitasOriginalValue
            => GetOriginalValue<int>(nameof(DashboardUltimasCitas));
        public bool DashboardUltimasCitasIsChanged
            => GetIsChanged(nameof(DashboardUltimasCitas));

        public int DashboardLongitudDeNombres
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }
        public int DashboardLongitudDeNombresOriginalValue
            => GetOriginalValue<int>(nameof(DashboardLongitudDeNombres));
        public bool DashboardLongitudDeNombresIsChanged
            => GetIsChanged(nameof(DashboardLongitudDeNombres));

        public int DashboardLongitudDeSeguimientos
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }
        public int DashboardLongitudDeSeguimientosOriginalValue
            => GetOriginalValue<int>(nameof(DashboardLongitudDeSeguimientos));
        public bool DashboardLongitudDeSeguimientosIsChanged
            => GetIsChanged(nameof(DashboardLongitudDeSeguimientos));

        public int DashboardMesesAMostrarDeAtencionesNuevas
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }
        public int DashboardMesesAMostrarDeAtencionesNuevasOriginalValue
            => GetOriginalValue<int>(nameof(DashboardMesesAMostrarDeAtencionesNuevas));
        public bool DashboardMesesAMostrarDeAtencionesNuevasIsChanged
            => GetIsChanged(nameof(DashboardMesesAMostrarDeAtencionesNuevas));

        public int DashboardMesesAMostrarDePersonasNuevas
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }
        public int DashboardMesesAMostrarDePersonasNuevasOriginalValue
            => GetOriginalValue<int>(nameof(DashboardMesesAMostrarDePersonasNuevas));
        public bool DashboardMesesAMostrarDePersonasNuevasIsChanged
            => GetIsChanged(nameof(DashboardMesesAMostrarDePersonasNuevas));

        ///
        /// Personas
        ///

        public int ListadoDePersonasItemsPerPage
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }
        public int ListadoDePersonasItemsPerPageOriginalValue
            => GetOriginalValue<int>(nameof(ListadoDePersonasItemsPerPage));
        public bool ListadoDePersonasItemsPerPageIsChanged
            => GetIsChanged(nameof(ListadoDePersonasItemsPerPage));

    }
}
