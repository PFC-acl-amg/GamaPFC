using Core;
using Gama.Atenciones.Wpf.Eventos;
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
    public class PreferenciasViewModel : ViewModelBase
    {
        public PreferenciasViewModel(IEventAggregator eventAggregator)
        {
            TestCommand = new DelegateCommand(OnTestCommandExecute);
        }

        private void OnTestCommandExecute()
        {
            MessageBox.Show("hh2");
        }

        public ICommand TestCommand { get; private set; }
    }
}
