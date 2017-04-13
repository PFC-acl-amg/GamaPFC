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

        public ICommand AceptarCommand { get; private set; }
        public ICommand CancelarCommand { get; private set; }

        //private Asistente _AsistenteSeleccionado;
        //public Asistente AsistenteSeleccionado
        //{
        //    get { return _AsistenteSeleccionado; }
        //    set
        //    {
        //        _AsistenteSeleccionado = value;
        //        OnPropertyChanged();
        //    }
        //}

        private Persona _PersonaSeleccionada;
        public Persona PersonaSeleccionada
        {
            get { return _PersonaSeleccionada; }
            set
            {
                _PersonaSeleccionada = value;
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

                Personas = new List<Persona>(_PersonaRepository.GetAll());
                OnPropertyChanged(nameof(Personas));

                if (EnEdicionDeCitaExistente)
                {
                    Cita.Asistente = Asistentes.Find(a => a.Id == Cita.Asistente.Id);
                }
            }
        }

        public List<Persona> Personas { get; private set; }

        public void LoadWithDefaultPerson()
        {
            //Persona = new PersonaWrapper(Personas.First());
            PersonaSeleccionada = Personas.First();
            Cita = new CitaWrapper(new Cita() { Persona = PersonaSeleccionada });
            Cita.PropertyChanged += Cita_PropertyChanged;

        }

        public void Load(PersonaWrapper persona)
        {
            Persona = persona;
            PersonaSeleccionada = Personas.Find(x => x.Id == persona.Id);
            Cita = new CitaWrapper(new Cita() { Persona = PersonaSeleccionada });
            Cita.PropertyChanged += Cita_PropertyChanged;
        }

        public void Load(Persona persona)
        {
            //Persona = new PersonaWrapper(persona);
            PersonaSeleccionada = Personas.Find(x => x.Id == persona.Id);
            Cita = new CitaWrapper(new Cita() { Persona = persona });
            Cita.PropertyChanged += Cita_PropertyChanged;
        }

        private void OnAceptarCommand_Execute()
        {
            if (!EnEdicionDeCitaExistente)
            {
                PersonaSeleccionada.AddCita(Cita.Model);
                if (Persona != null)
                    Persona.AddCita(Cita);
            }
            else
            {
                Cita citaActualizada = PersonaSeleccionada.Citas.Where(x => x.Id == Cita.Id).FirstOrDefault();
                citaActualizada.CopyValuesFrom(Cita.Model);
                if (Persona != null)
                {
                    CitaWrapper citaActualizada2 = Persona.Citas.Where(x => x.Id == Cita.Id).FirstOrDefault();
                    citaActualizada2.CopyValuesFrom(Cita.Model);
                }
            }

            #region Evitar problema de multiplicidad de entidades en NHibernate

            //if (Persona != null)
            //{
            //    List<Asistente> asistentes2 = Persona.Citas.Select(x => x.Asistente).Distinct().ToList();

            //    foreach (var cita in Persona.Citas)
            //    {
            //        cita.Asistente = asistentes2.Where(a => a.Id == cita.Asistente.Id).First();
            //    }

            //    asistentes2 = null;
            //}

            //List<Asistente> asistentes = PersonaSeleccionada.Citas.Select(x => x.Asistente).Distinct().ToList();

            //foreach (var cita in PersonaSeleccionada.Citas)
            //{
            //    cita.Asistente = asistentes.Where(a => a.Id == cita.Asistente.Id).First();
            //}

            //asistentes = null;

            #endregion
            if (Persona != null)
            {
                PersonaSeleccionada = null;
                _PersonaRepository.Update(Persona.Model);
                Persona.AcceptChanges();
            }
            else
                _PersonaRepository.Update(PersonaSeleccionada);

            //if (Persona != null)
            //    Persona.AcceptChanges();
            //PersonaSeleccionada.AcceptChanges();

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
            //Persona.RejectChanges();
            Cerrar = true;
        }
    }
}
