using Core;
using Gama.Cooperacion.Business;
using Gama.Cooperacion.Wpf.Eventos;
using Gama.Cooperacion.Wpf.Services;
using Gama.Cooperacion.Wpf.Wrappers;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gama.Cooperacion.Wpf.ViewModels
{
    public class NuevaActividadViewModel : ObservableObject
    {
        private IActividadRepository _actividadRepository;
        private ICooperanteRepository _cooperanteRepository;
        private IEventAggregator _eventAggregator;

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

            Actividad = new ActividadWrapper(new Actividad());

            CooperantesDisponibles = new ObservableCollection<CooperanteWrapper>(
                _cooperanteRepository.GetAll().Select(c => new CooperanteWrapper(c)));

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
        }

        public ActividadWrapper Actividad { get; set; }
        public CooperanteWrapper CooperanteSeleccionado { get; set; }
        public CooperanteWrapper CooperantePreviamenteSeleccionado { get; set; }
        public ObservableCollection<CooperanteWrapper> CooperantesDisponibles { get; private set; }

        public ICommand AceptarCommand { get; set; }
        public ICommand NuevoCooperanteCommand { get; set; }
        public ICommand CancelarCommand { get; private set; }
        public ICommand AbrirPopupCoordinadorCommand { get; private set; }
        public ICommand AbrirPopupCooperanteCommand { get; private set; }
        public ICommand AceptarNuevoCooperanteCommand { get; private set; }
        public ICommand QuitarCoordinadorCommand { get; private set; }
        public ICommand QuitarCooperanteCommand { get; private set; }

        private void OnAbrirPopupCoordinador(CooperanteWrapper cooperantePreviamenteSeleccionado)
        {
            PopupEstaAbierto = true;
            _modo = "Coordinador";
            CooperantePreviamenteSeleccionado = cooperantePreviamenteSeleccionado;
        }

        private void OnAbrirPopupCooperante(CooperanteWrapper cooperantePreviamenteSeleccionado)
        {
            PopupEstaAbierto = true;
            _modo = "Cooperante";

            CooperantePreviamenteSeleccionado = cooperantePreviamenteSeleccionado;
        }

        private bool OnNuevoCooperanteCommand_CanExecute() => CooperantesDisponibles.Count > 0;

        private void OnNuevoCooperanteCommand()
        {
            if (_modo == "Coordinador")
            {
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
                Actividad.Cooperantes.Insert(
                    Actividad.Cooperantes.IndexOf(CooperantePreviamenteSeleccionado),
                    CooperanteSeleccionado);

                // El nombre será nulo sólo en el caso del Cooperante Dummy
                if (CooperantePreviamenteSeleccionado.Nombre != null)
                {
                    CooperantesDisponibles.Add(CooperantePreviamenteSeleccionado);
                    Actividad.Cooperantes.Remove(CooperantePreviamenteSeleccionado);
                }

                Actividad.Model.AddCooperante(CooperanteSeleccionado.Model);
                //Actividad.Cooperantes.Add(new CooperanteWrapper(new Cooperante()));
                // TODO --> Meterlo en el SearchBox
                CooperantesDisponibles.Remove(CooperanteSeleccionado);

                // Si no quedan cooperantes disponibles, no se debe ver otra fila de 
                // inserción de cooperantes
                if (CooperantesDisponibles.Count == 0)
                {
                    Actividad.Cooperantes.Remove(Actividad.Cooperantes.Where(c => c.Nombre == null).First());
                }

                ((DelegateCommand<CooperanteWrapper>)QuitarCooperanteCommand).RaiseCanExecuteChanged();
            }

            CooperantePreviamenteSeleccionado = null;
            PopupEstaAbierto = false;
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
