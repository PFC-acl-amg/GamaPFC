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
        private IActividadRepository _actividadRepository;
        private bool? _cerrar;
        private ICooperanteRepository _cooperanteRepository;
        private IEventAggregator _eventAggregator;
        private IEnumerable _mensajeDeEspera;
        private string _modo;
        private bool _popupCoordinadorEstaAbierto;
        private IEnumerable _resultadoDeBusqueda;
        private LookupItem _selectedCooperante;
        private CooperanteWrapper _cooperanteDummy;

        public NuevaActividadViewModel(IActividadRepository actividadRepository,
            ICooperanteRepository cooperanteRepository,
            IEventAggregator eventAggregator)
        {
            _actividadRepository = actividadRepository;
            _cooperanteRepository = cooperanteRepository;
            _eventAggregator = eventAggregator;
            _mensajeDeEspera = new List<string>() { "Espera por favor..." };
            _cooperanteDummy = new CooperanteWrapper(new Cooperante());

            Actividad = new ActividadWrapper(new Actividad());

            CooperantesDisponibles = new ObservableCollection<CooperanteWrapper>(
                _cooperanteRepository.GetAll().Select(c => new CooperanteWrapper(c)));

            _resultadoDeBusqueda = new ObservableCollection<LookupItem>(
                CooperantesDisponibles.Select(c => new LookupItem
                {
                    Id = c.Id,
                    DisplayMember1 = c.NombreCompleto,
                    DisplayMember2 = c.Dni
                }));

            // Añadimos 'Cooperante Dummy' para que se muestre una fila del formulario para añadir el primero
            Actividad.Cooperantes.Add(_cooperanteDummy);
            
            AbrirPopupCommand = new DelegateCommand<CooperanteWrapper>(OnAbrirPopupCommand);
            AceptarCommand = new DelegateCommand(OnAceptarCommand, OnAceptarCommand_CanExecute);
            AceptarNuevoCooperanteCommand = new DelegateCommand(OnNuevoCooperanteCommand, OnNuevoCooperanteCommand_CanExecute);
            CancelarCommand = new DelegateCommand(OnCancelarCommand);
            QuitarCoordinadorCommand = new DelegateCommand(OnQuitarCoordinadorCommand, OnQuitarCoordinadorCommand_CanExecute);
            QuitarCooperanteCommand = new DelegateCommand<CooperanteWrapper>(OnQuitarCooperanteCommand, OnQuitarCooperanteCommand_CanExecute);
            SearchCommand = new DelegateCommand<string>(OnSearchEventCommand);
            SelectResultCommand = new DelegateCommand<CooperanteWrapper>(OnSelectResultEventCommand);
        }

        public bool? Cerrar
        {
            get { return _cerrar; }
            set { SetProperty(ref _cerrar, value); }
        }

        public bool PopupEstaAbierto
        {
            get { return _popupCoordinadorEstaAbierto; }
            set { SetProperty(ref _popupCoordinadorEstaAbierto, value); }
        }

        public LookupItem SelectedCooperante
        {
            get { return _selectedCooperante; }
            set { SetProperty(ref _selectedCooperante, value); }
        }

        public ActividadWrapper Actividad { get; set; }
        public ObservableCollection<CooperanteWrapper> CooperantesDisponibles { get; private set; }
        public CooperanteWrapper CooperantePreviamenteSeleccionado { get; set; }
        public CooperanteWrapper CooperanteSeleccionado { get; set; }
        public IEnumerable MensajeDeEspera => _mensajeDeEspera;
        public IEnumerable ResultadoDeBusqueda => _resultadoDeBusqueda;

        public ICommand AbrirPopupCommand { get; private set; }
        public ICommand AceptarCommand { get; set; }
        public ICommand AceptarNuevoCooperanteCommand { get; private set; }
        public ICommand CancelarCommand { get; private set; }
        public ICommand QuitarCooperanteCommand { get; private set; }
        public ICommand QuitarCoordinadorCommand { get; private set; }
        public ICommand SearchCommand { get; private set; }
        public ICommand SelectResultCommand { get; private set; }

        private void OnAbrirPopupCommand(CooperanteWrapper cooperanteAnterior)
        {
            PopupEstaAbierto = true;
            // Para el coordinador no hace falta que nos indiquen el anteriormente seleccionado
            // porque ya lo obtenemos de Actividad.Coordinador; Esto nos ahorra hace comprobaciones
            // para saber qué modo establecemos ya que el parámetro vendrá nulo. Esto se puede
            // ver desde la vista en Xaml. El comando para el botón de coordinador no 
            // indica ningún parámetro
            _modo = cooperanteAnterior == null ? "Coordinador" : "Cooperante";
            CooperantePreviamenteSeleccionado = cooperanteAnterior;
        }

        private void OnSearchEventCommand(string textoDeBusqueda)
        {
            _resultadoDeBusqueda = CooperantesDisponibles
                .Where(
                    c => c.Nombre.ToLower().Contains(textoDeBusqueda.Trim().ToLower()) ||
                         c.Apellido.ToLower().Contains(textoDeBusqueda.Trim().ToLower()))
                .Select(c => new LookupItem
                {
                    Id = c.Id,
                    DisplayMember1 = c.NombreCompleto,
                    DisplayMember2 = c.Dni
                });

            OnPropertyChanged("ResultadoDeBusqueda");
        }

        private void OnSelectResultEventCommand(CooperanteWrapper cooperanteAnterior)
        {
            var cooperanteNuevo = CooperantesDisponibles.Where(c => c.Id == SelectedCooperante.Id).First();
            InsertarCooperante(cooperanteAnterior, cooperanteNuevo);
        }

        private void OnNuevoCooperanteCommand()
        {
            if (_modo == "Coordinador")
            {
                // Es nulo cuando es el cooperante Dummy. Si no lo es,
                // añadimos el que ya estaba en la lista de los disponibles, ya
                // que se va a quitar
                if (Actividad.Coordinador.Nombre != null)
                {
                    CooperantesDisponibles.Add(Actividad.Coordinador);
                }

                Actividad.SetCoordinador(CooperanteSeleccionado);
                CooperantesDisponibles.Remove(CooperanteSeleccionado);
                ((DelegateCommand)QuitarCoordinadorCommand).RaiseCanExecuteChanged();
            }
            else if (_modo == "Cooperante")
            {
                InsertarCooperante(CooperantePreviamenteSeleccionado, CooperanteSeleccionado);
            }

            //CooperantePreviamenteSeleccionado = null;
            PopupEstaAbierto = false;
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

            Actividad.AddCooperante(cooperanteNuevo);
            CooperantesDisponibles.Remove(cooperanteNuevo);

            // Si quedan cooperantes disponibles y estamos añadiendo uno sin sustituir otro, 
            // hay que mostrar otra fila para añadir más cooperantes
            if (CooperantesDisponibles.Count > 0 && !Actividad.Cooperantes.Contains(_cooperanteDummy))
            {
                Actividad.Cooperantes.Add(_cooperanteDummy);
            }

            ((DelegateCommand<CooperanteWrapper>)QuitarCooperanteCommand).RaiseCanExecuteChanged();
        }

        private void OnQuitarCoordinadorCommand()
        {
            if (CooperantesDisponibles.Count == 0)
            {
                Actividad.Cooperantes.Add(_cooperanteDummy);
            }

            CooperantesDisponibles.Add(Actividad.Coordinador);
            Actividad.SetCoordinador(_cooperanteDummy);
            ((DelegateCommand)QuitarCoordinadorCommand).RaiseCanExecuteChanged();
        }

        private bool OnQuitarCoordinadorCommand_CanExecute()
        {
            return (Actividad.Coordinador.Nombre != null);
        }

        private void OnQuitarCooperanteCommand(CooperanteWrapper cooperante)
        {
            // Si antes de quitarlo no había más cooperantes disponibles, ahora
            // sí lo habrá así que incluímos el Dummy para poder añadirlo
            if (CooperantesDisponibles.Count == 0)
            {
                Actividad.Cooperantes.Add(_cooperanteDummy);
            }

            Actividad.Cooperantes.Remove(cooperante);
            CooperantesDisponibles.Add(cooperante);
        }

        private bool OnQuitarCooperanteCommand_CanExecute(CooperanteWrapper cooperante)
        {
            return (cooperante.Nombre != null);
        }

        private void OnAceptarCommand()
        {
            Actividad.Cooperantes.Remove(_cooperanteDummy);
            _actividadRepository.Create(Actividad.Model);
            _eventAggregator.GetEvent<NuevaActividadEvent>().Publish(Actividad.Id);
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
