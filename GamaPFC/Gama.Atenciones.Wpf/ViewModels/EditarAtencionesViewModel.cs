using Core;
using Gama.Atenciones.Business;
using Gama.Atenciones.Wpf.Eventos;
using Gama.Atenciones.Wpf.Services;
using Gama.Atenciones.Wpf.Views;
using Gama.Atenciones.Wpf.Wrappers;
using NHibernate;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gama.Atenciones.Wpf.ViewModels
{
    public class EditarAtencionesViewModel : ViewModelBase
    {
        private bool _VerAtenciones = false;
        private bool _EdicionHabilitada;
        private PersonaWrapper _Persona;
        private AtencionWrapper _AtencionSeleccionada;
        private IAtencionRepository _AtencionRepository;
        private ISession _Session;
        private IEventAggregator _EventAggregator;
        private IPersonaRepository _PersonaRepository;
        private IRegionManager _RegionManager;

        public EditarAtencionesViewModel(IAtencionRepository atencionRepository,
            IEventAggregator eventAggregator, IPersonaRepository personaRepository,
            ICitaRepository citaRepository,
            IRegionManager regionManager)
        {
            _EdicionHabilitada = true;
            Persona = new PersonaWrapper(new Persona());
            _AtencionRepository = atencionRepository;
            _PersonaRepository = personaRepository;
            _CitaRepository = citaRepository;
            _EventAggregator = eventAggregator;
            _RegionManager = regionManager;

            HabilitarEdicionCommand = new DelegateCommand(
                OnHabilitarEdicionCommandExecute,
                () => AtencionSeleccionada != null);

            ActualizarCommand = new DelegateCommand(
                OnActualizarCommandExecute,
                () => 
                      (AtencionSeleccionada != null && AtencionSeleccionada.IsChanged && AtencionSeleccionada.IsValid)
                   || (CitaSeleccionada     != null && CitaSeleccionada.IsChanged     && CitaSeleccionada.IsValid));

            CancelarEdicionCommand = new DelegateCommand(OnCancelarEdicionCommandExecute);

            EditarCitaCommand = new DelegateCommand(OnEditarCitaCommandExecute);

            PropertyChanged += EditarAtencionesViewModel_PropertyChanged;

            _EventAggregator.GetEvent<NuevaAtencionEvent>().Subscribe(OnNuevaAtencionEvent);
        }

        public ICommand HabilitarEdicionCommand { get; private set; }
        public ICommand ActualizarCommand { get; private set; }
        public ICommand CancelarEdicionCommand { get; private set; }
        public ICommand EditarCitaCommand { get; private set; }

        public ISession Session
        {
            get { return _Session; }
            set
            {
                _Session = value;
                _AtencionRepository.Session = _Session;
                _PersonaRepository.Session = _Session;
                _CitaRepository.Session = _Session;
            }
        }

        public bool VerAtenciones
        {
            get { return _VerAtenciones; }
            set
            {
                SetProperty(ref _VerAtenciones, value);
                if (AtencionSeleccionada == null && Atenciones.Count > 0)
                {
                    AtencionSeleccionada = Atenciones.First();
                }
            }
        }

        public PersonaWrapper Persona
        {
            get { return _Persona; }
            set { SetProperty(ref _Persona, value); }
        }

        public AtencionWrapper AtencionSeleccionada
        {
            get { return _AtencionSeleccionada; }
            set
            {
                SetProperty(ref _AtencionSeleccionada, value);
                CitaSeleccionada = new CitaWrapper(_AtencionSeleccionada.Cita);
            }
        }

        private CitaWrapper _CitaSeleccionada;
        private ICitaRepository _CitaRepository;

        public CitaWrapper CitaSeleccionada
        {
            get { return _CitaSeleccionada; }
            set { SetProperty(ref _CitaSeleccionada, value); }
        }

        public bool EdicionHabilitada
        {
            get { return _EdicionHabilitada; }
            set { SetProperty(ref _EdicionHabilitada, value); }
        }

        public ObservableCollection<AtencionWrapper> Atenciones { get; private set; }

        public void Load(PersonaWrapper wrapper)
        {
            EdicionHabilitada = false;
            Persona = wrapper;
            Atenciones = new ObservableCollection<AtencionWrapper>(
                Persona.Citas.Select(c => c.Atencion).Where(a => a != null && a.Id != 0).ToList());
            if (Atenciones.Count > 0)
            {
                AtencionSeleccionada = Atenciones.First();
            }
        }

        private void EditarAtencionesViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AtencionSeleccionada))
            {
                InvalidateCommands();
                AtencionSeleccionada.PropertyChanged += (s, ea) => { InvalidateCommands(); };
            }
            else if (e.PropertyName == nameof(CitaSeleccionada))
            {
                InvalidateCommands();
                CitaSeleccionada.PropertyChanged += (s, ea) => { InvalidateCommands(); };
            }
            else if (e.PropertyName == nameof(EdicionHabilitada))
            {
                InvalidateCommands();
            }
        }

        private void InvalidateCommands()
        {
            ((DelegateCommand)HabilitarEdicionCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)ActualizarCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)CancelarEdicionCommand).RaiseCanExecuteChanged();
        }

        private void OnNuevaAtencionEvent(CitaWrapper cita)
        {
            if (cita.Atencion == null) // atención nueva a crear
            {
                var atencion = new AtencionWrapper(new Atencion()
                {
                    Cita = cita.Model,
                    Fecha = DateTime.Now,
                    CreatedAt = DateTime.Now
                });

                cita.Atencion = atencion;
                _AtencionRepository.Create(atencion.Model);
                Atenciones.Add(atencion);
            }
            else // ya existe
            {
                AtencionSeleccionada = cita.Atencion;
            }

            // Navegamos a la pestaña de atenciones
            VerAtenciones = true;
        }

        private void OnHabilitarEdicionCommandExecute()
        {
            EdicionHabilitada = true;
        }

        private void OnActualizarCommandExecute()
        {
            if (AtencionSeleccionada != null && AtencionSeleccionada.IsChanged && AtencionSeleccionada.IsValid)
            {
                _AtencionRepository.Update(AtencionSeleccionada.Model);
            AtencionSeleccionada.AcceptChanges();
            }

            if (CitaSeleccionada != null && CitaSeleccionada.IsChanged && CitaSeleccionada.IsValid)
            {
                _CitaRepository.Update(CitaSeleccionada.Model);
                CitaSeleccionada.AcceptChanges();
            }

            EdicionHabilitada = false;
        }

        private void OnCancelarEdicionCommandExecute()
        {
            AtencionSeleccionada.RejectChanges();
            EdicionHabilitada = false;
        }

        private void OnEditarCitaCommandExecute()
        {
            var o = new NuevaCitaView() { Title = "Editar Cita" };
            var vm = (NuevaCitaViewModel)o.DataContext;
            vm.Session = _Session;
            vm.EnEdicionDeCitaExistente = true;
            vm.LoadForEdition(CitaSeleccionada);
            o.ShowDialog();
            Persona.AcceptChanges();
        }
    }
}
