using Core;
using Gama.Atenciones.Wpf.Eventos;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Atenciones.Wpf.ViewModels
{
    public class FlyoutsViewModel : ViewModelBase
    {
        private IEventAggregator _EventAggregator;
        private bool _PreferenciasFlyoutIsOpen = false;

        public FlyoutsViewModel(IEventAggregator eventAggregator)
        {
            _EventAggregator = eventAggregator;

            _EventAggregator.GetEvent<TogglePreferenciasEvent>().Subscribe(OnTogglePreferenciasEvent);
        }

        private void OnTogglePreferenciasEvent(bool isOpen)
        {
            PreferenciasFlyoutIsOpen = isOpen;
        }

        public bool PreferenciasFlyoutIsOpen
        {
            get { return _PreferenciasFlyoutIsOpen; }
            set { SetProperty(ref _PreferenciasFlyoutIsOpen, value); }
        }
    }
}
