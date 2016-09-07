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
        private ICooperanteRepository _cooperanteRepository;
        private IEventAggregator _eventAggregator;
        private IEnumerable _mensajeDeEspera;
        private IEnumerable _resultadoDeBusqueda;
        private string _textoDeBusqueda;

        private bool? _cerrar;
        public bool? Cerrar
        {
            get { return _cerrar; }
            set
            {
                _cerrar = value;
                OnPropertyChanged();
            }
        }

        private bool _popupCoordinadorEstaAbierto;
        private string _modo;

        public bool PopupEstaAbierto
        {
            get { return _popupCoordinadorEstaAbierto; }
            set
            {
                _popupCoordinadorEstaAbierto = value;
                OnPropertyChanged();
            }
        }

        public NuevaActividadViewModel(IActividadRepository actividadRepository,
            ICooperanteRepository cooperanteRepository,
            IEventAggregator eventAggregator)
        {
            _actividadRepository = actividadRepository;
            _cooperanteRepository = cooperanteRepository;
            _eventAggregator = eventAggregator;
            _mensajeDeEspera = new List<string>() { "Espera por favor..." };

            Actividad = new ActividadWrapper(new Actividad());

            CooperantesDisponibles = new ObservableCollection<CooperanteWrapper>(
                _cooperanteRepository.GetAll().Select(c => new CooperanteWrapper(c)));
            _resultadoDeBusqueda = new ObservableCollection<LookupItem>(
                CooperantesDisponibles.Select(c => new LookupItem
                {
                    DisplayMember1 = c.NombreCompleto,
                    DisplayMember2 = c.Dni
                }));

            // Añadimos 'Cooperante Dummy' para que siempre se muestre al menos
            // una fila del formulario para añadir uno más
            Actividad.Cooperantes.Add(new CooperanteWrapper(new Cooperante()));

            AceptarCommand = new DelegateCommand(OnAceptar, OnAceptar_CanExecute);
            NuevoCooperanteCommand = new DelegateCommand(OnNuevoCooperante);
            CancelarCommand = new DelegateCommand(OnCancelar);
            AbrirPopupCoordinadorCommand = new DelegateCommand<CooperanteWrapper>(OnAbrirPopupCoordinador);
            AbrirPopupCooperanteCommand = new DelegateCommand<CooperanteWrapper>(OnAbrirPopupCooperante);
            AceptarNuevoCooperanteCommand = 
                new DelegateCommand(OnNuevoCooperanteCommand, 
                                    OnNuevoCooperanteCommand_CanExecute);
            QuitarCoordinadorCommand = 
                new DelegateCommand(OnQuitarCoordinadorCommand,                                                       OnQuitarCoordinadorCommand_CanExecute);
            QuitarCooperanteCommand = 
                new DelegateCommand<CooperanteWrapper>(OnQuitarCooperanteCommand, OnQuitarCooperanteCommand_CanExecute);

            SearchCommand = new DelegateCommand<string>(OnSearch);
            SelectResultCommand = new DelegateCommand<CooperanteWrapper>(OnSelectResult);
            SelectionChangedCommand = new DelegateCommand<object[]>(OnSelectionChanged);

        }
        private LookupItem _selectedCooperante;
        public LookupItem SelectedCooperante
        {
            get { return _selectedCooperante; }
            set { SetProperty(ref _selectedCooperante, value); }
        }

        private void OnSelectionChanged(object[] items)
        {
            int i = 2;
        }

        public string TextoDeBusqueda
        {
            get { return _textoDeBusqueda; }
            set { SetProperty(ref _textoDeBusqueda, value); }
        }

        public IEnumerable MensajeDeEspera => _mensajeDeEspera;
        public IEnumerable ResultadoDeBusqueda => _resultadoDeBusqueda;
        public ActividadWrapper Actividad { get; set; }
        public CooperanteWrapper CooperanteSeleccionado { get; set; }
        public CooperanteWrapper CooperantePreviamenteSeleccionado { get; set; }
        public ObservableCollection<CooperanteWrapper> CooperantesDisponibles { get; private set; }
        public ObservableCollection<LookupItem> CooperantesForLookup { get; private set; }

        public ICommand AceptarCommand { get; set; }
        public ICommand NuevoCooperanteCommand { get; set; }
        public ICommand CancelarCommand { get; private set; }
        public ICommand AbrirPopupCoordinadorCommand { get; private set; }
        public ICommand AbrirPopupCooperanteCommand { get; private set; }
        public ICommand AceptarNuevoCooperanteCommand { get; private set; }
        public ICommand QuitarCoordinadorCommand { get; private set; }
        public ICommand QuitarCooperanteCommand { get; private set; }
        public ICommand SearchCommand { get; private set; }
        public ICommand SelectResultCommand { get; private set; }
        public ICommand SelectionChangedCommand { get; private set; }

        private void OnSearch(string textoDeBusqueda)
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

        private void OnSelectResult(CooperanteWrapper cooperanteAnterior)
        {
            var cooperanteNuevo = CooperantesDisponibles.Where(c => c.Id == SelectedCooperante.Id).First();
            InsertarCooperante(cooperanteAnterior, cooperanteNuevo);
        }

        private void OnAbrirPopupCoordinador(CooperanteWrapper cooperanteAnterior)
        {
            PopupEstaAbierto = true;
            _modo = "Coordinador";
            CooperantePreviamenteSeleccionado = cooperanteAnterior;
        }

        private void OnAbrirPopupCooperante(CooperanteWrapper cooperanteAnterior)
        {
            PopupEstaAbierto = true;
            _modo = "Cooperante";
            CooperantePreviamenteSeleccionado = cooperanteAnterior;
        }

        private bool OnNuevoCooperanteCommand_CanExecute() => CooperantesDisponibles.Count > 0;

        private void OnNuevoCooperanteCommand()
        {
            if (_modo == "Coordinador")
            {
                // Es nulo cuando es el cooperante Dummy
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

            Actividad.Model.AddCooperante(cooperanteNuevo.Model);
            CooperantesDisponibles.Remove(cooperanteNuevo);

            // Si quedan cooperantes disponibles, hay que mostrar otra fila
            // para añadir más cooperantes
            if (CooperantesDisponibles.Count > 0)
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
            Actividad.SetCoordinador(new CooperanteWrapper(new Cooperante()));
            ((DelegateCommand)QuitarCoordinadorCommand).RaiseCanExecuteChanged();
        }

        private bool OnQuitarCoordinadorCommand_CanExecute()
        {
            return (Actividad.Coordinador.Nombre != null);
        }

        private void OnQuitarCooperanteCommand(CooperanteWrapper cooperante)
        {
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

        private void OnAceptar()
        {
            _actividadRepository.Create(Actividad.Model);
            _eventAggregator.GetEvent<NuevaActividadEvent>().Publish(Actividad.Id);
            Cerrar = true;
        }

        private bool OnAceptar_CanExecute()
        {
            return true;  //(String.IsNullOrEmpty(Actividad.Titulo) &&
                          //String.IsNullOrEmpty(Actividad.Descripcion));
        }

        private void OnNuevoCooperante()
        {
            //CooperantesSeleccionados.Add(new Cooperante());
        }

        private void OnCancelar()
        {
            Cerrar = true;
        }
    }
}
