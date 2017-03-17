using Gama.Common.Eventos;
using Gama.Socios.Wpf.Eventos;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gama.Socios.Wpf.ViewModels
{
    public class RightCommandsViewModel
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
