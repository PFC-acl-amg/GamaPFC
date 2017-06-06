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

        public NuevaCitaViewModel(
            IPersonaRepository personaRepository,
            ICitaRepository citaRepository,
            IAsistenteRepository asistenteRepository,
            IEventAggregator eventAggregator,
            ISession session)
        {
            _PersonaRepository = personaRepository;
            _CitaRepository = citaRepository;
            _AsistenteRepository = asistenteRepository;
            _EventAggregator = eventAggregator;
            Session = session;

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

        private PersonaWrapper _PersonaSeleccionada;
        public PersonaWrapper PersonaSeleccionada
        {
            get { return _PersonaSeleccionada; }
            set
            {
                _PersonaSeleccionada = value;
                OnPropertyChanged();
            }
        }

        public List<Asistente> Asistentes { get; private set; }

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

        public bool EnEdicionDeCitaExistente { get; set; }

        public bool? Cerrar
        {
            get { return _Cerrar; }
            set { SetProperty(ref _Cerrar, value); }
        }

        private ISession _Session;
        public ISession Session
        {
            get { return _Session; }
            set
            {
                _Session = value;
                _PersonaRepository.Session = value;
                _CitaRepository.Session = value;
                _AsistenteRepository.Session = value;
            }
        }

        public List<PersonaWrapper> Personas { get; private set; }

        private void InicializarColecciones(bool incluirPersonas, Persona personaSeleccionada = null)
        {
            Asistentes = new List<Asistente>(_AsistenteRepository.GetAll());
            OnPropertyChanged(nameof(Asistentes));

            if (incluirPersonas)
            {
                //Personas = new List<Persona>(_PersonaRepository.GetAll());
                Personas = new List<PersonaWrapper>(_PersonaRepository.Personas.Select(x => new PersonaWrapper(x)).ToList());
                OnPropertyChanged(nameof(Personas));
            }
            else
            {
                Personas = new List<PersonaWrapper>() { new PersonaWrapper(personaSeleccionada) };
                OnPropertyChanged(nameof(Personas));
            }

            if (EnEdicionDeCitaExistente)
                AsistenteSeleccionado = Asistentes.Find(a => a.Id == Cita.Asistente.Id);
            else
                AsistenteSeleccionado = Asistentes.First();
        }

        public void LoadWithDefaultPerson()
        {
            InicializarColecciones(incluirPersonas: true);
            PersonaSeleccionada = Personas.First();
            Cita = new CitaWrapper(new Cita() {
                Persona = PersonaSeleccionada.Model,
                Asistente = AsistenteSeleccionado
            });
            Cita.PropertyChanged += Cita_PropertyChanged;
        }

        /// <summary>
        /// Este Load se usa para editar una cita existente.
        /// </summary>
        /// <param name="citaExistente">Cita existente a editar.</param>
        public void LoadForEdition(CitaWrapper citaExistente)
        {
            Cita = citaExistente;
           
            Cita.PropertyChanged += Cita_PropertyChanged;
            InicializarColecciones(incluirPersonas: false, personaSeleccionada: citaExistente.Persona);
            PersonaSeleccionada = Personas.Find(x => x.Id == citaExistente.Persona.Id);

            ((DelegateCommand)AceptarCommand).RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Este Load se usa para crear una nueva cita.
        /// </summary>
        /// <param name="persona">Persona a la que asignarle la nueva cita.</param>
        /// <param name="fechaSeleccionada">Fecha seleccionada inicial (se puede modificar).</param>
        public void LoadForCreation(PersonaWrapper persona, DateTime fechaSeleccionada)
        {
            InicializarColecciones(incluirPersonas: false, personaSeleccionada: persona.Model);

            PersonaSeleccionada = persona;

            Cita = new CitaWrapper(new Cita()
            {
                Persona = persona.Model,
                Fecha = fechaSeleccionada,
                Asistente = AsistenteSeleccionado
            });

            Cita.PropertyChanged += Cita_PropertyChanged;
        }

        private void OnAceptarCommand_Execute()
        {
            Cita.Fecha = Cita.Fecha.Value.Date.AddHours(Cita.Hora).AddMinutes(Cita.Minutos);
            Cita.Asistente = AsistenteSeleccionado;

            if (!EnEdicionDeCitaExistente)
            {
                Cita.Persona = PersonaSeleccionada.Model;
                _CitaRepository.Create(Cita.Model);
            }
            else
            {
                Cita citaActualizada = PersonaSeleccionada.Citas.Where(x => x.Id == Cita.Id).FirstOrDefault().Model;
                citaActualizada.CopyValuesFrom(Cita.Model);
                _CitaRepository.Update(Cita.Model);
            }

            Cita.AcceptChanges();

            Cerrar = true;
        }

        private bool OnAceptarCommand_CanExecute()
        {
            if (Cita == null) return false;

            return Cita.IsValid;
        }

        private void OnCancelarCommand_Execute()
        {
            Cita.RejectChanges();
            Cerrar = true;
        }
    }
}
