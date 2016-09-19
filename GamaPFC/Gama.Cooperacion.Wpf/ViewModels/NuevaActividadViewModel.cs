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
        private bool? _Cerrar; // Debe ser nulo al inicializarse el VM, o hay excepción con Dialogcloser
        private IEventAggregator _EventAggregator;
        private InformacionDeActividadViewModel _ActividadVM;

        public NuevaActividadViewModel(IActividadRepository actividadRepository,
            IEventAggregator eventAggregator,
            InformacionDeActividadViewModel actividadViewModel,
            ISession session)
        {
            _ActividadRepository = actividadRepository;
            _ActividadRepository.Session = session;
            _EventAggregator = eventAggregator;
            _ActividadVM = actividadViewModel;
            _ActividadVM._ActividadRepository = _ActividadRepository;
           
            AceptarCommand = new DelegateCommand(OnAceptarCommand, OnAceptarCommand_CanExecute);
            CancelarCommand = new DelegateCommand(OnCancelarCommand);
        }

        public InformacionDeActividadViewModel ActividadVM
        {
            get { return _ActividadVM; }
        }

        public bool? Cerrar
        {
            get { return _Cerrar; }
            set { SetProperty(ref _Cerrar, value); }
        }

        public ICommand AceptarCommand { get; set; }
        public ICommand CancelarCommand { get; private set; }

        private void OnAceptarCommand()
        {
            _ActividadVM.Actividad.Cooperantes.Remove(_ActividadVM.Actividad.Cooperantes.Where(c => c.Nombre == null).First());
            foreach (var cooperante in _ActividadVM.Actividad.Cooperantes)
                cooperante.Model.ActividadesEnQueParticipa.Add(_ActividadVM.Actividad.Model);

            _ActividadRepository.Session.Clear();
            _ActividadRepository.Create(_ActividadVM.Actividad.Model);
            _EventAggregator.GetEvent<NuevaActividadEvent>().Publish(_ActividadVM.Actividad.Id);
            Cerrar = true;
        }

        private bool OnAceptarCommand_CanExecute()
        {
            return true; // _ActividadVM.Actividad.Coordinador.Nombre != null;
        }

        private void OnCancelarCommand()
        {
            Cerrar = true;
        }
    }
}
