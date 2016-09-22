using Core;
using Gama.Common.CustomControls;
using Gama.Cooperacion.Business;
using Gama.Cooperacion.Wpf.Eventos;
using Gama.Cooperacion.Wpf.Services;
using Gama.Cooperacion.Wpf.Wrappers;
using NHibernate;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Gama.Cooperacion.Wpf.ViewModels
{
    public class NuevaActividadViewModel : ViewModelBase
    {
        private IActividadRepository _ActividadRepository;
        private bool? _Cerrar; // Debe ser nulo al inicializarse el VM, o hay excepción con DialogCloser
        private IEventAggregator _EventAggregator;
        private InformacionDeActividadViewModel _ActividadVM;

        public NuevaActividadViewModel(IActividadRepository actividadRepository,
            IEventAggregator eventAggregator,
            InformacionDeActividadViewModel actividadViewModel,
            ISession session)
        {
            _EventAggregator = eventAggregator;
            _ActividadVM = actividadViewModel;
            _ActividadRepository = actividadRepository;
            _ActividadRepository.Session = session;
            _ActividadVM._ActividadRepository = _ActividadRepository;
            _ActividadVM.Actividad.PropertyChanged += Actividad_PropertyChanged;

            AceptarCommand = new DelegateCommand(OnAceptarCommand, OnAceptarCommand_CanExecute);
            CancelarCommand = new DelegateCommand(OnCancelarCommand);
        }

        private void Actividad_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_ActividadVM.Actividad.Coordinador))
            {
                ((DelegateCommand)AceptarCommand).RaiseCanExecuteChanged();
            }
        }

        public InformacionDeActividadViewModel ActividadVM
        {
            get { return _ActividadVM; }
        }

        // Para cerrar la ventana
        public bool? Cerrar
        {
            get { return _Cerrar; }
            set { SetProperty(ref _Cerrar, value); }
        }

        public ICommand AceptarCommand { get; set; }
        public ICommand CancelarCommand { get; private set; }

        private void OnAceptarCommand()
        {
            // Eliminamos el cooperante dummy
            _ActividadVM.Actividad.Cooperantes.Remove(_ActividadVM.Actividad.Cooperantes.Where(c => c.Nombre == null).First());
            _ActividadRepository.Create(_ActividadVM.Actividad.Model);
            _EventAggregator.GetEvent<NuevaActividadEvent>().Publish(_ActividadVM.Actividad.Id);
            Cerrar = true;
        }

        private bool OnAceptarCommand_CanExecute()
        {
            var c = _ActividadVM.Actividad;
            var resultado = c.Titulo != null && c.Coordinador.Nombre != null;

            return resultado;
        }

        private void OnCancelarCommand()
        {
            Cerrar = true;
        }
    }
}
