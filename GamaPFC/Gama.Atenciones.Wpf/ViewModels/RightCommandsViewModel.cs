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

        public RightCommandsViewModel(IEventAggregator eventAggregator)
        {
            _EventAggregator = eventAggregator;

            AbrirPreferenciasCommand = new DelegateCommand(OnAbrirPreferenciasCommandExecute);
        }

        public ICommand AbrirPreferenciasCommand { get; private set; }

        private void OnAbrirPreferenciasCommandExecute()
        {
            _EventAggregator.GetEvent<AbrirPreferenciasEvent>().Publish();
        }
    }
}
