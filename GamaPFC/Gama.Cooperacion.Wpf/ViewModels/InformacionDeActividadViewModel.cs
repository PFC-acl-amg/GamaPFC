using Core;
using Gama.Common.CustomControls;
using Gama.Cooperacion.Business;
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
        private bool _EdicionHabilitada;
        private ActividadWrapper _Actividad;
        public IActividadRepository _ActividadRepository { get; set; }
        private ICooperanteRepository _CooperanteRepository;
        private IEventAggregator _EventAggregator;
        private IEnumerable _MensajeDeEspera;
        private string _Modo;
        private bool _PopupEstaAbierto = false;
        private IEnumerable _ResultadoDeBusqueda;
        private LookupItem _CooperanteBuscado;
        private LookupItem _CoordinadorBuscado;

        public InformacionDeActividadViewModel(
            ICooperanteRepository cooperanteRepository,
            IEventAggregator eventAggregator, ISession session)
        {
            _CooperanteRepository = cooperanteRepository;
            _CooperanteRepository.Session = session;
            _EventAggregator = eventAggregator;
            _MensajeDeEspera = new List<string>() { "Espera por favor..." };
            _EdicionHabilitada = true;

            Actividad = new ActividadWrapper(new Actividad());

            CooperantesDisponibles = new ObservableCollection<CooperanteWrapper>(
                _CooperanteRepository.GetAll().Select(c => new CooperanteWrapper(c)));

            _ResultadoDeBusqueda = new ObservableCollection<LookupItem>(
                CooperantesDisponibles.Select(c => new LookupItem
                {
                    Id = c.Id,
                    DisplayMember1 = c.NombreCompleto,
                    DisplayMember2 = c.Dni
                }));

            // Añadimos 'Cooperante Dummy' para que se muestre una fila del formulario para añadir el primero
            Actividad.Cooperantes.Add(new CooperanteWrapper(new Cooperante()));

            AbrirPopupCommand = new DelegateCommand<CooperanteWrapper>(OnAbrirPopupCommand);
            NuevoCooperanteCommand = new DelegateCommand(OnNuevoCooperanteCommand, OnNuevoCooperanteCommand_CanExecute);
            QuitarCoordinadorCommand = new DelegateCommand(OnQuitarCoordinadorCommand, OnQuitarCoordinadorCommand_CanExecute);
            QuitarCooperanteCommand = new DelegateCommand<CooperanteWrapper>(OnQuitarCooperanteCommand, OnQuitarCooperanteCommand_CanExecute);
            SearchCommand = new DelegateCommand<string>(OnSearchEventCommand);
            SelectResultCommand = new DelegateCommand<CooperanteWrapper>(OnSelectResultEventCommand);
            SelectCoordinadorCommand = new DelegateCommand(OnSelectCoordinadorEventCommand);
        }

        public bool EdicionHabilitada
        {
            get { return _EdicionHabilitada; }
            set { SetProperty(ref _EdicionHabilitada, value); }
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
            set
            {
                _Actividad = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<CooperanteWrapper> CooperantesDisponibles { get; private set; }
        public CooperanteWrapper CooperantePreviamenteSeleccionado { get; set; }
        public CooperanteWrapper CooperanteEmergenteSeleccionado { get; set; }
        public IEnumerable MensajeDeEspera => _MensajeDeEspera;
        public IEnumerable ResultadoDeBusqueda => _ResultadoDeBusqueda;

        public ICommand AbrirPopupCommand { get; private set; }
        public ICommand NuevoCooperanteCommand { get; private set; }
        public ICommand QuitarCooperanteCommand { get; private set; }
        public ICommand QuitarCoordinadorCommand { get; private set; }
        public ICommand SearchCommand { get; private set; }
        public ICommand SelectResultCommand { get; private set; }
        public ICommand SelectCoordinadorCommand { get; private set; }

        public void InicializarParaVer(ActividadWrapper wrapper)
        {
            EdicionHabilitada = false;
            Actividad = wrapper;
            foreach (var cooperante in Actividad.Cooperantes)
            {
                CooperantesDisponibles.Remove(CooperantesDisponibles.Where(c => c.Id == cooperante.Id).First());
            }
            CooperantesDisponibles.Remove(CooperantesDisponibles.Where(c => c.Id == Actividad.Coordinador.Id).First());
            Actividad.Cooperantes.Add(new CooperanteWrapper(new Cooperante()));
            Actividad.AcceptChanges();
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

        private void OnSelectResultEventCommand(CooperanteWrapper cooperanteAnterior)
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

        private void EstablecerCoordinador(CooperanteWrapper coordinadorSeleccionado)
        {
            // Es nulo cuando es el cooperante Dummy. Si no lo es,
            // añadimos el que ya estaba en la lista de los disponibles, ya
            // que se va a quitar
            if (Actividad.Coordinador.Nombre != null)
            {
                CooperantesDisponibles.Add(Actividad.Coordinador);
            }

            Actividad.Coordinador = coordinadorSeleccionado;

            CooperanteEmergenteSeleccionado = null;
            CooperanteBuscado = null;
            CooperantePreviamenteSeleccionado = null;

            CooperantesDisponibles.Remove(coordinadorSeleccionado);
            ((DelegateCommand)QuitarCoordinadorCommand).RaiseCanExecuteChanged();
        }

        private bool OnNuevoCooperanteCommand_CanExecute()
        {
            return CooperantesDisponibles.Count > 0;
        }

        private void InsertarCooperante(
            CooperanteWrapper cooperanteAnterior,
            CooperanteWrapper cooperanteNuevo)
        {
            Actividad.Cooperantes[Actividad.Cooperantes.IndexOf(cooperanteAnterior)]
                = cooperanteNuevo;

            // El nombre será nulo sólo en el caso del Cooperante Dummy
            if (cooperanteAnterior.Nombre != null)
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
            if (CooperantesDisponibles.Count == 0)
            {
                Actividad.Cooperantes.Add(new CooperanteWrapper(new Cooperante()));
            }

            CooperantesDisponibles.Add(Actividad.Coordinador);
            Actividad.Coordinador = new CooperanteWrapper(new Cooperante());
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
            if (CooperantesDisponibles.Count == 0)
            {
                Actividad.Cooperantes.Add(new CooperanteWrapper(new Cooperante()));
            }

            Actividad.Cooperantes.Remove(cooperante);
            CooperantesDisponibles.Add(cooperante);
        }

        private bool OnQuitarCooperanteCommand_CanExecute(CooperanteWrapper cooperante)
        {
            return (cooperante.Nombre != null);
        }
    }
}