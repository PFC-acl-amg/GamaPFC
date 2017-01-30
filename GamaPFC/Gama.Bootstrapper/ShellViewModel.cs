using Core;
using Gama.Atenciones.Wpf.Eventos;
using Prism.Events;
using System.Windows.Media;
using System;

namespace Gama.Bootstrapper
{
    public class ShellViewModel : ViewModelBase
    {
        private IEventAggregator _EventAggregator;
        private ImageSource _IconSource;
        private bool _PreferenciasFlyoutIsOpen = false;

        public ShellViewModel(IEventAggregator eventAggregator)
        {
            _EventAggregator = eventAggregator;
            Title = "Módulo no cargado";
            _EventAggregator.GetEvent<AbrirPreferenciasEvent>().Subscribe(OnTogglePreferenciasEvent);
        }

        private void OnTogglePreferenciasEvent()
        {
            PreferenciasFlyoutIsOpen = true;
        }

        public bool PreferenciasFlyoutIsOpen
        {
            get { return _PreferenciasFlyoutIsOpen; }
            set { SetProperty(ref _PreferenciasFlyoutIsOpen, value); }
        }

        public ImageSource IconSource
        {
            get { return _IconSource; }
            set { SetProperty(ref _IconSource, value); }
        }
    }
}