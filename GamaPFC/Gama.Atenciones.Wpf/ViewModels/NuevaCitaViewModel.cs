using Core;
using Gama.Atenciones.Business;
using Gama.Atenciones.Wpf.Services;
using Gama.Atenciones.Wpf.Wrappers;
using Gama.Atenciones.Wpf.Eventos;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using NHibernate;
using Prism.Events;

namespace Gama.Atenciones.Wpf.ViewModels
{
    public class NuevaCitaViewModel : ViewModelBase
    {
        private bool? _Cerrar; // Debe ser nulo al inicializarse el VM o hay excepciones por DialogCloser
        private IPersonaRepository _PersonaRepository;
        private ICitaRepository _CitaRepository;
        private IEventAggregator _EventAggregator;
        private IAsistenteRepository _AsistenteRepository;

        public NuevaCitaViewModel(IPersonaRepository personaRepository, 
            ICitaRepository citaRepository, 
            IAsistenteRepository asistenteRepository,
            IEventAggregator eventAggregator)
        {
            _PersonaRepository = personaRepository;
            _CitaRepository = citaRepository;
            _AsistenteRepository = asistenteRepository;
            _EventAggregator = eventAggregator;
            
            AceptarCommand = new DelegateCommand(OnAceptarCommand_Execute,
                OnAceptarCommand_CanExecute);
            CancelarCommand = new DelegateCommand(OnCancelarCommand_Execute);
        }

        private void Cita_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Cita.Atencion));
            ((DelegateCommand)AceptarCommand).RaiseCanExecuteChanged();
        }

        private Asistente _AsistenteSeleccionado;
        public Asistente AsistenteSeleccionado
        {
            get { return _AsistenteSeleccionado; }
            set
            {
                _AsistenteSeleccionado = value;
                OnPropertyChanged();
            }
        }

        public List<Asistente> Asistentes { get; private set; }

        private CitaWrapper _Cita;
        public CitaWrapper Cita
        {
            get { return _Cita; }
            set
            {
                _Cita = value;
                //Cita.PropertyChanged += Cita_PropertyChanged;
                OnPropertyChanged(nameof(Cita));
            }
        }
        public PersonaWrapper Persona { get; private set; }

        public bool EnEdicionDeCitaExistente { get; set; }

        public bool? Cerrar
        {
            get { return _Cerrar; }
            set { SetProperty(ref _Cerrar, value); }
        }

        public ICommand AceptarCommand { get; private set; }
        public ICommand CancelarCommand { get; private set; }

        public ISession Session
        {
            get { return null; }
            set
            {
                _PersonaRepository.Session = value;
                _CitaRepository.Session = value;
                _AsistenteRepository.Session = value;

                Asistentes = new List<Asistente>(_AsistenteRepository.GetAll());
                OnPropertyChanged(nameof(Asistentes));
            }
        }

        public void Load(PersonaWrapper persona)
        {
            Persona = persona;
            Cita = new CitaWrapper(new Cita() { Persona = Persona.Model });
            Cita.PropertyChanged += Cita_PropertyChanged;
        }

        private void OnAceptarCommand_Execute()
        {
            if (!EnEdicionDeCitaExistente)
                Persona.Citas.Add(Cita);
            else
            {
                CitaWrapper citaActualizada = Persona.Citas.Where(x => x.Id == Cita.Id).FirstOrDefault();
                citaActualizada.CopyValuesFrom(Cita.Model);
            }

            List<Asistente> asistentes = Persona.Citas.Select(x => x.Asistente).Distinct().ToList();

            foreach(var cita in Persona.Citas)
            {
                cita.Asistente = asistentes.Where(a => a.Id == cita.Asistente.Id).First();
            }

            asistentes = new List<Asistente>();

            _PersonaRepository.Update(Persona.Model);
            //_CitaRepository.Create(Cita.Model);
            Persona.AcceptChanges();

            if (!EnEdicionDeCitaExistente)
                _EventAggregator.GetEvent<CitaCreadaEvent>().Publish(Cita.Id);
            else
                _EventAggregator.GetEvent<CitaActualizadaEvent>().Publish(Cita.Id);

            Cerrar = true;
        }

        private bool OnAceptarCommand_CanExecute()
        {
            if (Cita == null) return false;

            return Cita.IsValid;
        }

        private void OnCancelarCommand_Execute()
        {
            Persona.RejectChanges();
            Cerrar = true;
        }
    }
}
