using Core;
using Gama.Socios.Wpf.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Socios.Wpf.Wrappers
{
    public class PreferenciasWrapper : ModelWrapper<Preferencias>
    {
        public PreferenciasWrapper(Preferencias model) : base(model)
        {

        }

     
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
        public bool CuotaMensualPredeterminadaIsChanged => GetIsChanged(nameof(CuotaMensualPredeterminada));  ///
                                                                                                              /// General
                                                                                                              /// 

        public bool General_EdicionHabilitadaPorDefecto
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }
        public bool General_EdicionHabilitadaPorDefectoOriginalValue => GetOriginalValue<bool>(nameof(General_EdicionHabilitadaPorDefecto));
        public bool General_EdicionHabilitadaPorDefectoIsChanged => GetIsChanged(nameof(General_EdicionHabilitadaPorDefecto));

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

        public bool DoBackupOnClose
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }
        public bool DoBackupOnCloseOriginalValue => GetOriginalValue<bool>(nameof(DoBackupOnClose));
        public bool DoBackupOnCloseIsChanged => GetIsChanged(nameof(DoBackupOnClose));

        public DateTime? BackupDeleteDateLimit
        {
            get { return GetValue<DateTime?>(); }
            set { SetValue(value); }
        }
        public bool BackupDeleteDateLimitOriginalValue => GetOriginalValue<bool>(nameof(BackupDeleteDateLimit));
        public bool BackupDeleteDateLimitIsChanged => GetIsChanged(nameof(BackupDeleteDateLimit));
    }
}
