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

namespace Gama.Atenciones.Wpf.ViewModels
{
    public class EditarPersonaViewModel : ViewModelBase, IConfirmNavigationRequest, IActiveAware
    {
        private IEventAggregator _EventAggregator;
        private IPersonaRepository _PersonaRepository;
        private PersonaViewModel _PersonaVM;
        private EditarAtencionesViewModel _AtencionesVM;
        private EditarCitasViewModel _CitasVM;

        public EditarPersonaViewModel(
            IEventAggregator eventAggregator,
            IPersonaRepository personaRepository,
            PersonaViewModel personaVM,
            EditarAtencionesViewModel atencionesVM,
            EditarCitasViewModel citasVM,
            ISession session)
        {
            _EventAggregator = eventAggregator;
            _PersonaRepository = personaRepository;
            _PersonaRepository.Session = session;
            _PersonaVM = personaVM;
            _AtencionesVM = atencionesVM;
            _AtencionesVM.Session = session;
            _CitasVM = citasVM;
            _CitasVM.Session = session;

            CitasIsVisible = true;
            AtencionesIsVisible = false;

            HabilitarEdicionCommand = new DelegateCommand(
                OnHabilitarEdicionCommand,
                () => !_PersonaVM.EdicionHabilitada);
            ActualizarCommand = new DelegateCommand(
                OnActualizarCommand,
                () =>
                _PersonaVM.EdicionHabilitada
                   && Persona.IsChanged
                   && Persona.IsValid
                   );
            CancelarEdicionCommand = new DelegateCommand(OnCancelarEdicionCommand,
                () => _PersonaVM.EdicionHabilitada);
            EliminarPersonaCommand = new DelegateCommand(OnEliminarPersonaCommandExecute);
            ActivarVistaCommand = new DelegateCommand<string>(OnActivarVistaCommandExecute);

            _PersonaVM.PropertyChanged += _PersonaVM_PropertyChanged;
        }

        private void OnActivarVistaCommandExecute(string param)
        {
            CitasIsVisible = param == "citas";
            AtencionesIsVisible = param == "atenciones";
        }

        public PersonaViewModel PersonaVM               => _PersonaVM;
        public EditarAtencionesViewModel AtencionesVM   => _AtencionesVM;
        public EditarCitasViewModel CitasVM             => _CitasVM;
        public PersonaWrapper Persona                   => _PersonaVM.Persona;

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
                    _EventAggregator.GetEvent<PersonaSeleccionadaChangedEvent>().Publish(Persona.Id);
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
            _PersonaRepository.Update(Persona.Model);
            _PersonaVM.Persona.AcceptChanges();
            _PersonaVM.EdicionHabilitada = false;
            RefrescarTitulo(Persona.Nombre);
            _EventAggregator.GetEvent<PersonaActualizadaEvent>().Publish(Persona.Id);
        }

        private void OnHabilitarEdicionCommand()
        {
            _PersonaVM.EdicionHabilitada = true;
        }

        private void OnCancelarEdicionCommand()
        {
            Persona.RejectChanges();
            _PersonaVM.EdicionHabilitada = false;
        }

        private void OnEliminarPersonaCommandExecute()
        {
            var o = new ConfirmarOperacionView();
            o.Mensaje = "¿Está seguro de que desea eliminar esta persona y todos sus registros?";
            o.ShowDialog();

            if (o.EstaConfirmado)
            {
                int id = Persona.Id;
                // WARNING: Debe hacer antes la publicación del evento porque se recoge
                // la persona para ver sus citas y atenciones desde otros viewmodels
                //_EventAggregator.GetEvent<PersonaEliminadaEvent>().Publish(id);
                _PersonaRepository.Delete(Persona.Model);
            }
        }

        public void Load(int id)
        {
            try
            {
                if (Persona.Nombre != null)
                    return;

                var personaModel = _PersonaRepository.GetById(id);
                var persona = new PersonaWrapper(personaModel);

                _PersonaVM.Load(persona);
                _AtencionesVM.Load(_PersonaVM.Persona);
                _CitasVM.Load(_PersonaVM.Persona);
                RefrescarTitulo(persona.Nombre);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool IsNavigationTarget(int id)
        {
            return (Persona.Id == id);
        }

        public override void OnActualizarServidor()
        {
            if (!Persona.IsChanged)
            {
                var persona = new PersonaWrapper(
                    (Persona)
                    _PersonaRepository.GetById(Persona.Id)
                    .DecryptFluent());

                _PersonaVM.Load(persona);
                _AtencionesVM.Load(_PersonaVM.Persona);
                _CitasVM.Load(_PersonaVM.Persona);
                RefrescarTitulo(persona.Nombre);
                _AtencionesVM.VerAtenciones = false;
            }
        }

        public void OnNavigatedTo(int personaId, int? atencionId = null)
        {
            try
            {
                if (Persona.Nombre == null)
                {
                    var persona = new PersonaWrapper(
                        (Persona)
                        _PersonaRepository.GetById(personaId)
                        .DecryptFluent());

                    _PersonaVM.Load(persona);
                    _AtencionesVM.Load(_PersonaVM.Persona);
                    _CitasVM.Load(_PersonaVM.Persona);
                    RefrescarTitulo(persona.Nombre);
                    _AtencionesVM.VerAtenciones = false;
                }
                
                if (atencionId.HasValue)
                {
                    _AtencionesVM.EdicionHabilitada = false;

                    if (_AtencionesVM.Atenciones != null)
                        _AtencionesVM.AtencionSeleccionada = _AtencionesVM.Atenciones.Where(x => x.Id == atencionId.Value).FirstOrDefault();

                    AtencionesIsVisible = true;
                    CitasIsVisible = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AddCita(Cita cita)
        {
            //Persona.Citas.Add(new CitaWrapper(cita));
            Persona.AcceptChanges();
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return IsNavigationTarget((int)navigationContext.Parameters["Id"]);
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            OnNavigatedTo((int)navigationContext.Parameters["Id"], (int?)navigationContext.Parameters["AtencionId"]);
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
            if (e.PropertyName == nameof(_PersonaVM.EdicionHabilitada))
            {
                InvalidateCommands();
            }
            else if (e.PropertyName == nameof(Persona))
            {
                Persona.PropertyChanged += (s, ea) => {
                    if (ea.PropertyName == nameof(Persona.IsChanged)
                        || ea.PropertyName == nameof(Persona.IsValid))
                    {
                        InvalidateCommands();
                    }
                };
            }
        }

        public bool ConfirmNavigationRequest()
        {
            if (Persona.IsChanged)
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
            if (Persona.IsChanged)
            {
                var o = new ConfirmarOperacionView();
                o.Mensaje = "Si sale se perderán los cambios, ¿Desea salir de todas formas?";
                o.ShowDialog();

                continuationCallback.Invoke(o.EstaConfirmado);
            }
        }
    }
}
