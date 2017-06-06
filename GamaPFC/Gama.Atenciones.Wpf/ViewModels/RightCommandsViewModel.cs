using Core;
using Core.Util;
using Gama.Atenciones.Wpf.Eventos;
using Gama.Common.Eventos;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Gama.Atenciones.Wpf.ViewModels
{
    public class RightCommandsViewModel
    {
        private IEventAggregator _EventAggregator;

        public RightCommandsViewModel(IEventAggregator eventAggregator)
        {
            _EventAggregator = eventAggregator;

            AbrirPreferenciasCommand = new DelegateCommand(OnAbrirPreferenciasCommandExecute);
            VolverASeleccionDeModuloCommand = new DelegateCommand(OnVolverASeleccionDeModuloExecute);
        }

        public ICommand AbrirPreferenciasCommand { get; private set; }
        public ICommand VolverASeleccionDeModuloCommand { get; private set; }

        private void OnAbrirPreferenciasCommandExecute()
        {
            _EventAggregator.GetEvent<AbrirPreferenciasEvent>().Publish();
        }

        private void OnVolverASeleccionDeModuloExecute()
        {
            UIServices.RestartApplication();
        }
    }
}
