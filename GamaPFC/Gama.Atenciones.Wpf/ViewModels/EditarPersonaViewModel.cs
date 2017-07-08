using Core;
using Gama.Atenciones.Business;
using Gama.Atenciones.Wpf.Eventos;
using Gama.Atenciones.Wpf.Services;
using Gama.Atenciones.Wpf.Wrappers;
using Gama.Common.Views;
using NHibernate;
using Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Gama.Common.Eventos;
using Core.Util;

namespace Gama.Atenciones.Wpf.ViewModels
{
    public class EditarPersonaViewModel : ViewModelBase, IConfirmNavigationRequest, IActiveAware
    {
        private IEventAggregator _EventAggregator;
        private IPersonaRepository _PersonaRepository;
        private PersonaViewModel _PersonaVM;
        private EditarAtencionesViewModel _AtencionesVM;
        private EditarCitasViewModel _CitasVM;
        private Preferencias _Preferencias;

        public EditarPersonaViewModel(
            IEventAggregator eventAggregator,
            IPersonaRepository personaRepository,
            PersonaViewModel personaVM,
            EditarAtencionesViewModel atencionesVM,
            EditarCitasViewModel citasVM,
            ISession session,
            Preferencias preferencias)
        {
            _EventAggregator = eventAggregator;
            _PersonaRepository = personaRepository;
            _PersonaRepository.Session = session;
            _PersonaVM = personaVM;
            _AtencionesVM = atencionesVM;
            _AtencionesVM.Session = session;
            _CitasVM = citasVM;
            _CitasVM.Session = session;

            _Preferencias = preferencias;

            CitasIsVisible = true;
            AtencionesIsVisible = false;

            HabilitarEdicionCommand = new DelegateCommand(
                OnHabilitarEdicionCommand,
                () => !PersonaVM.Persona.IsInEditionMode);
            ActualizarCommand = new DelegateCommand(
                OnActualizarCommand,
                () =>
                PersonaVM.Persona.IsInEditionMode
                   && PersonaVM.Persona.IsChanged
                   && PersonaVM.Persona.IsValid
                   );
            CancelarEdicionCommand = new DelegateCommand(OnCancelarEdicionCommand,
                () => _PersonaVM.Persona.IsInEditionMode);
            EliminarPersonaCommand = new DelegateCommand(OnEliminarPersonaCommandExecute);
            ActivarVistaCommand = new DelegateCommand<string>(OnActivarVistaCommandExecute);
        }

        private void OnActivarVistaCommandExecute(string param)
        {
            CitasIsVisible = param == "citas";
            AtencionesIsVisible = param == "atenciones";
        }

        public PersonaViewModel PersonaVM               => _PersonaVM;
        public EditarAtencionesViewModel AtencionesVM   => _AtencionesVM;
        public EditarCitasViewModel CitasVM             => _CitasVM;

        public ICommand HabilitarEdicionCommand { get; private set; }
        public ICommand ActualizarCommand { get; private set; }
        public ICommand CancelarEdicionCommand { get; private set; }
        public ICommand EliminarPersonaCommand { get; private set; }
        public ICommand ActivarVistaCommand { get; private set; }
        
        private bool _IsActive;
        public bool IsActive
        {
            get { return _IsActive; }

            set
            {
                SetProperty(ref _IsActive, value);
                if (_IsActive)
                    _EventAggregator.GetEvent<PersonaSeleccionadaChangedEvent>().Publish(PersonaVM.Persona.Id);
            }
        }

        private bool _CitasIsVisible;
        public bool CitasIsVisible
        {
            get { return _CitasIsVisible; }
            set { SetProperty(ref _CitasIsVisible, value); }
        }

        private bool _AtencionesIsVisible;
        public bool AtencionesIsVisible
        {
            get { return _AtencionesIsVisible; }
            set { SetProperty(ref _AtencionesIsVisible, value); }
        }

        private void OnActualizarCommand()
        {
            UIServices.SetBusyState();
            if (_PersonaVM.Persona.ImagenIsChanged)
                _PersonaVM.Persona.ImagenUpdatedAt = DateTime.Now;

            _PersonaRepository.Update(_PersonaVM.Persona.Model);
            _PersonaVM.Persona.AcceptChanges();
            _PersonaVM.Persona.IsInEditionMode = false;
            RefrescarTitulo(_PersonaVM.Persona.Nombre);
        }

        private void OnHabilitarEdicionCommand()
        {
            _PersonaVM.Persona.IsInEditionMode = true;
        }

        private void OnCancelarEdicionCommand()
        {
            _PersonaVM.Persona.RejectChanges();
            _PersonaVM.Persona.IsInEditionMode = false;
        }

        private void OnEliminarPersonaCommandExecute()
        {
            var o = new ConfirmarOperacionView();
            o.Mensaje = "¿Está seguro de que desea eliminar esta persona y todos sus registros?";
            o.ShowDialog();

            if (o.EstaConfirmado)
            {
                int id = _PersonaVM.Persona.Id;
                // WARNING: Debe hacer antes la publicación del evento porque se recoge
                // la persona para ver sus citas y atenciones desde otros viewmodels
                //_EventAggregator.GetEvent<PersonaEliminadaEvent>().Publish(id);
                _PersonaRepository.Delete(_PersonaVM.Persona.Model);
            }
        }

        public bool IsNavigationTarget(int id)
        {
            return (_PersonaVM.Persona.Id == id);
        }

        public override void OnActualizarServidor()
        {
            if (!_PersonaVM.Persona.IsChanged)
            {
                var model =
                    (Persona)
                    _PersonaRepository.GetById(_PersonaVM.Persona.Id)
                    .DecryptFluent();
                var persona = new PersonaWrapper(model);

                _PersonaVM.Load(persona);
                _AtencionesVM.Load(_PersonaVM.Persona);
                _CitasVM.Load(_PersonaVM.Persona);
                RefrescarTitulo(persona.Nombre);
                _AtencionesVM.VerAtenciones = false;
            }
        }

        public void OnNavigatedTo(int personaId, int? atencionId = null, DateTime? fechaDecita = null)
        {
            try
            {
                if (string.IsNullOrEmpty(_PersonaVM.Persona.Nombre))
                {
                    var model =
                        (Persona)
                        _PersonaRepository.GetById(personaId)
                        .DecryptFluent();
                    var persona = new PersonaWrapper(model);
                    _PersonaVM.Load(persona);
                    _PersonaVM.Persona.IsInEditionMode = _Preferencias.General_EdicionHabilitadaPorDefecto;
                    _AtencionesVM.Load(_PersonaVM.Persona);
                    _CitasVM.Load(_PersonaVM.Persona);
                    RefrescarTitulo(persona.Nombre);
                    _AtencionesVM.VerAtenciones = false;
                    InvalidateCommands();
                }
                
                if (atencionId.HasValue)
                {
                    //_AtencionesVM.EdicionHabilitada = false;
                    _AtencionesVM.EdicionHabilitada = _Preferencias.General_EdicionHabilitadaPorDefecto;

                    if (_AtencionesVM.Atenciones != null)
                        _AtencionesVM.AtencionSeleccionada = _AtencionesVM.Atenciones.Where(x => x.Id == atencionId.Value).FirstOrDefault();

                    AtencionesIsVisible = true;
                    CitasIsVisible = false;
                }

                if (fechaDecita.HasValue)
                {
                    _CitasVM.CurrentDate = fechaDecita.Value;
                }

                _PersonaVM.PropertyChanged += _PersonaVM_PropertyChanged;
                _PersonaVM.Persona.PropertyChanged += _PersonaVM_PropertyChanged;
                PropertyChanged += _PersonaVM_PropertyChanged;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void RefrescarTitulo(string nombre)
        {
            if (nombre.Length > 20)
            {
                Title = nombre.Substring(0, 20) + "...";
            }
            else
            {
                Title = nombre;
            }
        }

        private void InvalidateCommands()
        {
            ((DelegateCommand)HabilitarEdicionCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)ActualizarCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)CancelarEdicionCommand).RaiseCanExecuteChanged();
        }

        private void _PersonaVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            InvalidateCommands();
        }

        public bool ConfirmNavigationRequest()
        {
            if (_PersonaVM.Persona.IsChanged)
            {
                var o = new ConfirmarOperacionView();
                o.Mensaje = "Si sale se perderán los cambios, ¿Desea salir de todas formas?";
                o.ShowDialog();

                return o.EstaConfirmado;
            }

            return true;
        }

        public void ConfirmNavigationRequest(NavigationContext navigationContext,
            Action<bool> continuationCallback)
        {
            if (_PersonaVM.Persona.IsChanged)
            {
                var o = new ConfirmarOperacionView();
                o.Mensaje = "Si sale se perderán los cambios, ¿Desea salir de todas formas?";
                o.ShowDialog();

                continuationCallback.Invoke(o.EstaConfirmado);
            }
        }
    }
}
