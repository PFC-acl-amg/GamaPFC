using Core;
using Gama.Cooperacion.Wpf.Eventos;
using Gama.Cooperacion.Wpf.Views;
using NHibernate;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gama.Cooperacion.Wpf.ViewModels
{
    public class ToolbarViewModel : ViewModelBase
    {
        private IEventAggregator _eventAggregator;

        public ToolbarViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            NuevaActividadCommand = new DelegateCommand(OnNuevaActividad);
        }

        public ICommand NuevaActividadCommand { get; private set; }

        private void OnNuevaActividad()
        {
            var o = new NuevaActividadView();
            o.ShowDialog();
        }
    }
}
