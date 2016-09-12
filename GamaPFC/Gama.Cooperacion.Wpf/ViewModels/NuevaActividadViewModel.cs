using Core;
using Gama.Common.CustomControls;
using Gama.Cooperacion.Business;
using Gama.Cooperacion.Wpf.Eventos;
using Gama.Cooperacion.Wpf.Services;
using Gama.Cooperacion.Wpf.Wrappers;
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
        private ICooperanteRepository _CooperanteRepository;
        private IEventAggregator _EventAggregator;
        private ActividadInformacionBasicaViewModel _ActividadVM;

        public NuevaActividadViewModel(IActividadRepository actividadRepository,
            ICooperanteRepository cooperanteRepository,
            IEventAggregator eventAggregator,
            ActividadInformacionBasicaViewModel actividadViewModel)
        {
            _ActividadRepository = actividadRepository;
            _CooperanteRepository = cooperanteRepository;
            _EventAggregator = eventAggregator;
            _ActividadVM = actividadViewModel;
           
            AceptarCommand = new DelegateCommand(OnAceptarCommand, OnAceptarCommand_CanExecute);
            CancelarCommand = new DelegateCommand(OnCancelarCommand);
        }

        public ActividadInformacionBasicaViewModel ActividadVM
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
            _ActividadRepository.Create(_ActividadVM.Actividad.Model);
            _EventAggregator.GetEvent<NuevaActividadEvent>().Publish(_ActividadVM.Actividad.Id);
            Cerrar = true;
        }

        private bool OnAceptarCommand_CanExecute()
        {
            return true;  //(String.IsNullOrEmpty(Actividad.Titulo) &&
                          //String.IsNullOrEmpty(Actividad.Descripcion));
        }

        private void OnCancelarCommand()
        {
            Cerrar = true;
        }
    }
}
