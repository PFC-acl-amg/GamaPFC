using Core;
using Gama.Atenciones.Wpf.Eventos;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gama.Atenciones.Wpf.ViewModels
{
    public class RightCommandsViewModel : ViewModelBase
    {
        private IEventAggregator _EventAggregator;
        private bool _PrefenciasIsOpen = false;

        public RightCommandsViewModel(IEventAggregator eventAggregator)
        {
            _EventAggregator = eventAggregator;

            TogglePreferenciasCommand = new DelegateCommand(OnTogglePreferenciasCommandExecute);
        }

        public ICommand TogglePreferenciasCommand { get; private set; }

        private void OnTogglePreferenciasCommandExecute()
        {
            _EventAggregator.GetEvent<TogglePreferenciasEvent>().Publish(!_PrefenciasIsOpen);
            _PrefenciasIsOpen = !_PrefenciasIsOpen;
        }
    }
}
