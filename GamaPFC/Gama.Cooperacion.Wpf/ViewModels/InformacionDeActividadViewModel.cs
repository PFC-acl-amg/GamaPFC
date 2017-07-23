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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gama.Cooperacion.Wpf.ViewModels
{
    public class InformacionDeActividadViewModel : ViewModelBase
    {
        private ActividadWrapper _Actividad;
        public IActividadRepository _ActividadRepository { get; set; }
        private ICooperanteRepository _CooperanteRepository;
        private IEventAggregator _EventAggregator;
        private IEnumerable _MensajeDeEspera;
        private DateTime? _FechaInicioActividad;
        private DateTime? _FechaFinActividad;
        private string _EstadoActividad;
        private string _EstadoEscogido;
        private string _Modo;
        private bool _PopupEstaAbierto = false;
        private IEnumerable _ResultadoDeBusqueda;
        private LookupItem _CooperanteBuscado;
        private LookupItem _CoordinadorBuscado;
        private ISession _Session;
        private ICooperanteRepository _cooperanteRepositoryNuevaTarea;

        public InformacionDeActividadViewModel(
            IActividadRepository actividadRepository,
            ICooperanteRepository cooperanteRepository,
            IEventAggregator eventAggregator,
            ISession sessionNuevaTarea)
        {
            _ActividadRepository = actividadRepository;
            _CooperanteRepository = cooperanteRepository;
            _cooperanteRepositoryNuevaTarea = cooperanteRepository;
            _EventAggregator = eventAggregator;
            _MensajeDeEspera = new List<string>() { "Espera por favor..." };

            _cooperanteRepositoryNuevaTarea.Session = sessionNuevaTarea;
            if (CooperantesDisponibles == null)
            {
                CooperantesDisponibles = new ObservableCollection<CooperanteWrapper>(
                    _cooperanteRepositoryNuevaTarea.GetAll().Select(c => new CooperanteWrapper(c)));

                _ResultadoDeBusqueda = new ObservableCollection<LookupItem>(
                    CooperantesDisponibles.Select(c => new LookupItem
                    {
                        Id = c.Id,
                        DisplayMember1 = c.NombreCompleto,
                        DisplayMember2 = c.Dni
                    }));
            }
            sessionNuevaTarea.Close();
            Actividad = new ActividadWrapper(new Actividad() { Titulo = "", Descripcion = "" });
            EstadosValidos = new List<string>();
            EstadosValidos.Add(Estado.Comenzado.ToString());
            EstadosValidos.Add(Estado.NoComenzado.ToString());
            EstadosValidos.Add(Estado.ProximasFinalizaciones.ToString());
            EstadosValidos.Add(Estado.FueraPlazo.ToString());
            EstadosValidos.Add(Estado.Finalizado.ToString());

            // Añadimos 'Cooperante Dummy' para que se muestre una fila del formulario para añadir el primero
            Actividad.Cooperantes.Add(new CooperanteWrapper(new Cooperante()));
            Actividad.FechaDeFin = DateTime.Today;
            Actividad.FechaDeInicio = DateTime.Today;

            _EventAggregator.GetEvent<ActividadActualizadaEvent>().Subscribe(OnActividadActualizadaEvent);

            AbrirPopupCommand = new DelegateCommand<CooperanteWrapper>(OnAbrirPopupCommand);
            NuevoCooperanteCommand = new DelegateCommand(OnNuevoCooperanteCommand, OnNuevoCooperanteCommand_CanExecute);
            QuitarCoordinadorCommand = new DelegateCommand(OnQuitarCoordinadorCommand, OnQuitarCoordinadorCommand_CanExecute);
            QuitarCooperanteCommand = new DelegateCommand<CooperanteWrapper>(OnQuitarCooperanteCommand, OnQuitarCooperanteCommand_CanExecute);
            SearchCommand = new DelegateCommand<string>(OnSearchEventCommand);
            SelectCooperanteEventCommand = new DelegateCommand<CooperanteWrapper>(OnSelectCooperanteEventCommand);
            SelectCoordinadorCommand = new DelegateCommand(OnSelectCoordinadorEventCommand);
        }
        private void OnActividadActualizadaEvent(Actividad ActividadActualizada)
        {
            var Act = new ActividadWrapper(ActividadActualizada);
            Actividad = Act;
            Actividad.IsInEditionMode = false;
        }
        public string EstadoEscogido
        {
            get { return _EstadoEscogido; }
            set { SetProperty(ref _EstadoEscogido, value); }
        }
        public string EstadoActividad
        {
            get { return _EstadoActividad; }
            set { SetProperty(ref _EstadoActividad, value); }
        }
        public DateTime? FechaInicioActividad
        {
            get { return _FechaInicioActividad; }
            set { SetProperty(ref _FechaInicioActividad, value); }
        }
        public DateTime? FechaFinActividad
        {
            get { return _FechaFinActividad; }
            set { SetProperty(ref _FechaFinActividad, value); }
        }

        // Se usa para abrir el Popup de la lista emergente de cooperantes
        public bool PopupEstaAbierto
        {
            get { return _PopupEstaAbierto; }
            set { SetProperty(ref _PopupEstaAbierto, value); }
        }

        // El que se enlaza con el elemento seleccionado del SearchBox
        public LookupItem CooperanteBuscado
        {
            get { return _CooperanteBuscado; }
            set { SetProperty(ref _CooperanteBuscado, value); }
        }

        // El que se enlaza con el elemento seleccionado del SearchBox
        public LookupItem CoordinadorBuscado
        {
            get { return _CoordinadorBuscado; }
            set { SetProperty(ref _CoordinadorBuscado, value); }
        }

        public ActividadWrapper Actividad
        {
            get { return _Actividad; }
            set { SetProperty(ref _Actividad, value); }
        }

        public ISession Session
        {
            get { return _Session;  }
            set
            {
                _Session = value;
                _ActividadRepository.Session = _Session;
                _CooperanteRepository.Session = _Session;
            }
        }

        public ObservableCollection<CooperanteWrapper> CooperantesDisponibles { get; private set; }
        public CooperanteWrapper CooperantePreviamenteSeleccionado { get; set; }
        public CooperanteWrapper CooperanteEmergenteSeleccionado { get; set; }
        public List<string> EstadosValidos { get; set; }
        public IEnumerable MensajeDeEspera => _MensajeDeEspera;
        public IEnumerable ResultadoDeBusqueda => _ResultadoDeBusqueda;

        public ICommand AbrirPopupCommand { get; private set; }
        public ICommand NuevoCooperanteCommand { get; private set; }
        public ICommand QuitarCooperanteCommand { get; private set; }
        public ICommand QuitarCoordinadorCommand { get; private set; }
        public ICommand SearchCommand { get; private set; }
        public ICommand SelectCooperanteEventCommand { get; private set; }
        public ICommand SelectCoordinadorCommand { get; private set; }

        public void Load(ActividadWrapper wrapper)
        {
            if (CooperantesDisponibles == null)
            {
                CooperantesDisponibles = new ObservableCollection<CooperanteWrapper>(
                    _CooperanteRepository.GetAll().Select(c => new CooperanteWrapper(c)));

            _ResultadoDeBusqueda = new ObservableCollection<LookupItem>(
                CooperantesDisponibles.Select(c => new LookupItem
                {
                    Id = c.Id,
                    DisplayMember1 = c.NombreCompleto,
                    DisplayMember2 = c.Dni
                }));
            }

            Actividad = wrapper;
            Actividad.PropertyChanged += (s, e) => {
                if (e.PropertyName == "IsChanged")
                    InvalidateCommands();
            };
            // Se prepara la lista de CooperantesDisponibles recorriendo los cooperantes de la actividad.
            // Cuando lea el dumy no hara nada porque este varlo no tiene porque estar en los cooperantes disponibles
            foreach (var cooperante in Actividad.Cooperantes)
            {
                CooperantesDisponibles.Remove(CooperantesDisponibles.Where(c => c.Id == cooperante.Id).FirstOrDefault());
            }
            CooperantesDisponibles.Remove(CooperantesDisponibles.Where(c => c.Id == Actividad.Coordinador.Id).FirstOrDefault());
            Actividad.AcceptChanges();
            OnPropertyChanged(nameof(Actividad));
        }

        private void OnAbrirPopupCommand(CooperanteWrapper cooperanteAnterior)
        {
            PopupEstaAbierto = true;
            // Para el coordinador no hace falta que nos indiquen el anteriormente seleccionado
            // porque ya lo obtenemos de Actividad.Coordinador; Esto nos ahorra hacer comprobaciones
            // para saber qué modo establecemos ya que el parámetro vendrá nulo. Esto se puede
            // ver desde la vista en Xaml. El comando para el botón de coordinador no 
            // indica ningún parámetro
            _Modo = cooperanteAnterior == null ? "Coordinador" : "Cooperante";
            CooperantePreviamenteSeleccionado = cooperanteAnterior;
        }

        private void OnSearchEventCommand(string textoDeBusqueda)
        {
            _ResultadoDeBusqueda = CooperantesDisponibles
                .Where(
                    c => c.Nombre != null &&
                         (c.Nombre.ToLower().Contains(textoDeBusqueda.Trim().ToLower()) ||
                         c.Apellido.ToLower().Contains(textoDeBusqueda.Trim().ToLower())))
                .Select(c => new LookupItem
                {
                    Id = c.Id,
                    DisplayMember1 = c.NombreCompleto,
                    DisplayMember2 = c.Dni
                });

            OnPropertyChanged("ResultadoDeBusqueda");
        }

        private void OnSelectCoordinadorEventCommand()
        {
            var coordinadorNuevo = CooperantesDisponibles.Where(c => c.Id == CoordinadorBuscado.Id).First();
            EstablecerCoordinador(coordinadorNuevo);
        }

        private void OnSelectCooperanteEventCommand(CooperanteWrapper cooperanteAnterior)
        {
            var cooperanteNuevo = CooperantesDisponibles.Where(c => c.Id == CooperanteBuscado.Id).First();
            InsertarCooperante(cooperanteAnterior, cooperanteNuevo);
        }

        // Viene por aquí si es desde el popup emergente, y no desde el SearchBox
        private void OnNuevoCooperanteCommand()
        {
            PopupEstaAbierto = false;
            if (_Modo == "Coordinador")
            {
                EstablecerCoordinador(CooperanteEmergenteSeleccionado);
            }
            else if (_Modo == "Cooperante")
            {
                InsertarCooperante(CooperantePreviamenteSeleccionado, CooperanteEmergenteSeleccionado);
            }
        }

        private void InvalidateCommands()
        {
            ((DelegateCommand)QuitarCoordinadorCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)NuevoCooperanteCommand).RaiseCanExecuteChanged();
            ((DelegateCommand<CooperanteWrapper>)QuitarCooperanteCommand).RaiseCanExecuteChanged();
        }

        private void EstablecerCoordinador(CooperanteWrapper coordinadorSeleccionado)
        {
            // Es nulo cuando es el cooperante Dummy. Si no lo es,
            // añadimos el que ya estaba en la lista de los disponibles, ya
            // que se va a quitar
            if (Actividad.Coordinador.Nombre != "")
            {
                CooperantesDisponibles.Add(Actividad.Coordinador);
            }

            Actividad.Coordinador = coordinadorSeleccionado;

            CooperanteEmergenteSeleccionado = null;
            CooperanteBuscado = null;
            CooperantePreviamenteSeleccionado = null;

            CooperantesDisponibles.Remove(coordinadorSeleccionado);
            ((DelegateCommand)QuitarCoordinadorCommand).RaiseCanExecuteChanged(); // Para que es
        }

        private bool OnNuevoCooperanteCommand_CanExecute()
        {
            if (CooperantesDisponibles == null)
                return false;

            return CooperantesDisponibles.Count > 0;
        }

        private void InsertarCooperante(
            CooperanteWrapper cooperanteAnterior,
            CooperanteWrapper cooperanteNuevo)
        {
            var cooperanteAReemplazar = Actividad.Cooperantes.Single(c => c.Id == cooperanteAnterior.Id);
            int indiceAReemplazar = Actividad.Cooperantes.IndexOf(cooperanteAReemplazar);
            
            Actividad.Cooperantes.Insert(indiceAReemplazar, cooperanteNuevo);
            Actividad.Cooperantes.RemoveAt(indiceAReemplazar + 1);

            // El nombre será nulo sólo en el caso del Cooperante Dummy
            if (cooperanteAnterior.Nombre != "")
            {
                CooperantesDisponibles.Add(cooperanteAnterior);
            }

            //Actividad.AddCooperante(cooperanteNuevo);
            CooperantesDisponibles.Remove(cooperanteNuevo);

            // Si quedan cooperantes disponibles y estamos añadiendo uno sin sustituir otro, 
            // hay que mostrar otra fila para añadir más cooperantes
            if (CooperantesDisponibles.Count > 0 && Actividad.Cooperantes.Where(c => c.Nombre == null).ToList().Count == 0)
            {
                Actividad.Cooperantes.Add(new CooperanteWrapper(new Cooperante()));
            }

            ((DelegateCommand<CooperanteWrapper>)QuitarCooperanteCommand).RaiseCanExecuteChanged();
        }

        private void OnQuitarCoordinadorCommand()
        {
            // Al quitar al coordinador podemos ponerlo como cooperante
            if (CooperantesDisponibles.Count == 0)
            {
                Actividad.Cooperantes.Add(new CooperanteWrapper(new Cooperante()));
            }

            CooperantesDisponibles.Add(Actividad.Coordinador);  // Añade al cooerdinador saliente a cooperantees disponibles
            Actividad.Coordinador = new CooperanteWrapper(new Cooperante()); // El nuevo coordinador es el dumy
            ((DelegateCommand)QuitarCoordinadorCommand).RaiseCanExecuteChanged();
        }

        private bool OnQuitarCoordinadorCommand_CanExecute()
        {
           return (Actividad.Coordinador.Nombre != null && !string.IsNullOrEmpty(Actividad.Coordinador.Nombre));
        }

        private void OnQuitarCooperanteCommand(CooperanteWrapper cooperante)
        {
            // Si antes de quitarlo no había más cooperantes disponibles, ahora
            // sí lo habrá así que incluímos el Dummy para poder añadirlo

            // Cuando se esta modificando no esta disponible el dumy.
            // => si quitas el último cooperante no podras meter ninguno nuevo
            // Cuando estas modicicando.
            // Controlamos si se va a quitar el ultimo cooperante para añadir el dumy.
            if (Actividad.Cooperantes.Count==1) Actividad.Cooperantes.Add(new CooperanteWrapper(new Cooperante()));
            if (CooperantesDisponibles.Count == 0)
            {
                Actividad.Cooperantes.Add(new CooperanteWrapper(new Cooperante()));
            }

            Actividad.Cooperantes.Remove(cooperante);
            CooperantesDisponibles.Add(cooperante);
        }

        private bool OnQuitarCooperanteCommand_CanExecute(CooperanteWrapper cooperante)
        {
            return (cooperante.Nombre != "");
        }
    }
}