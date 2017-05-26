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
            {
                Cita.Asistente = Asistentes.Find(a => a.Id == Cita.Asistente.Id);
            }
        }

        public void LoadWithDefaultPerson()
        {
            InicializarColecciones(incluirPersonas: true);
            //Persona = new PersonaWrapper(Personas.First());
            PersonaSeleccionada = Personas.First();
            Cita = new CitaWrapper(new Cita() { Persona = PersonaSeleccionada.Model });
            Cita.PropertyChanged += Cita_PropertyChanged;
        }

        /// <summary>
        /// Este Load se usa para editar una cita existente.
        /// </summary>
        /// <param name="citaExistente">Cita existente a editar.</param>
        public void LoadForEdition(CitaWrapper citaExistente)
        {
            Cita = citaExistente;
            //if (Cita != null)
            //{
                Cita.PropertyChanged += Cita_PropertyChanged;
                InicializarColecciones(incluirPersonas: false, personaSeleccionada: citaExistente.Persona);
                PersonaSeleccionada = Personas.Find(x => x.Id == citaExistente.Persona.Id);
            //}
        }

        /// <summary>
        /// Este Load se usa para crear una nueva cita.
        /// </summary>
        /// <param name="persona">Persona a la que asignarle la nueva cita.</param>
        /// <param name="fechaSeleccionada">Fecha seleccionada inicial (se puede modificar).</param>
        public void LoadForCreation(PersonaWrapper persona, DateTime fechaSeleccionada)
        {
            InicializarColecciones(incluirPersonas: false, personaSeleccionada: persona.Model);
            //Persona = persona;
            PersonaSeleccionada = persona;

            Cita = new CitaWrapper(new Cita()
            {
                Persona = persona.Model,
                Fecha = fechaSeleccionada
            });

            Cita.PropertyChanged += Cita_PropertyChanged;
        }

        //public void LoadForCreation(int personaId, DateTime fechaSeleccionada)
        //{
        //    Persona persona = _PersonaRepository.GetById(personaId);

        //    InicializarColecciones(incluirPersonas: false, personaSeleccionada: persona);
        //    //Persona = persona;
        //    PersonaSeleccionada = new PersonaWrapper(persona);

        //    Cita = new CitaWrapper(new Cita()
        //    {
        //        Persona = persona,
        //        Fecha = fechaSeleccionada
        //    });

        //    Cita.PropertyChanged += Cita_PropertyChanged;
        //}

        private void OnAceptarCommand_Execute()
        {
            Cita.Fecha = Cita.Fecha.Value.AddHours(Cita.Hora).AddMinutes(Cita.Minutos);

            if (!EnEdicionDeCitaExistente)
            {
                Cita.Persona = PersonaSeleccionada.Model;
                    //PersonaSeleccionada.AddCita(Cita.Model);
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

        //private void OnAceptarCommand_Execute()
        //{
        //    if (!EnEdicionDeCitaExistente)
        //    {
        //        PersonaSeleccionada.AddCita(Cita.Model);
        //        if (Persona != null)
        //            Persona.AddCita(Cita.Model);
        //    }
        //    else
        //    {
        //        Cita citaActualizada = PersonaSeleccionada.Citas.Where(x => x.Id == Cita.Id).FirstOrDefault();
        //        citaActualizada.CopyValuesFrom(Cita.Model);
        //        if (Persona != null)
        //        {
        //            CitaWrapper citaActualizada2 = Persona.Citas.Where(x => x.Id == Cita.Id).FirstOrDefault();
        //            citaActualizada2.CopyValuesFrom(Cita.Model);
        //        }
        //    }

        //    #region Evitar problema de multiplicidad de entidades en NHibernate

        //    //if (Persona != null)
        //    //{
        //    //    List<Asistente> asistentes2 = Persona.Citas.Select(x => x.Asistente).Distinct().ToList();

        //    //    foreach (var cita in Persona.Citas)
        //    //    {
        //    //        cita.Asistente = asistentes2.Where(a => a.Id == cita.Asistente.Id).First();
        //    //    }

        //    //    asistentes2 = null;
        //    //}

        //    //List<Asistente> asistentes = PersonaSeleccionada.Citas.Select(x => x.Asistente).Distinct().ToList();

        //    //foreach (var cita in PersonaSeleccionada.Citas)
        //    //{
        //    //    cita.Asistente = asistentes.Where(a => a.Id == cita.Asistente.Id).First();
        //    //}

        //    //asistentes = null;

        //    #endregion
        //    if (Persona != null)
        //    {
        //        PersonaSeleccionada = null;
        //        _PersonaRepository.Update(Persona.Model);
        //        Persona.AcceptChanges();
        //    }
        //    else
        //        _PersonaRepository.Update(PersonaSeleccionada);

        //    //if (Persona != null)
        //    //    Persona.AcceptChanges();
        //    //PersonaSeleccionada.AcceptChanges();

        //    if (!EnEdicionDeCitaExistente)
        //        _EventAggregator.GetEvent<CitaCreadaEvent>().Publish(Cita.Id);
        //    else
        //        _EventAggregator.GetEvent<CitaActualizadaEvent>().Publish(Cita.Id);

        //    Cerrar = true;
        //}
    }
}
