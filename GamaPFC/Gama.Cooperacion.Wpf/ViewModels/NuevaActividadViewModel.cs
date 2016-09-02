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
            IEventAggregator eventAggregator)
        {
            _actividadRepository = actividadRepository;
            _eventAggregator = eventAggregator;

            Actividad = new ActividadWrapper(new Actividad());

            CooperantesDisponibles = new ObservableCollection<CooperanteWrapper>();
            CooperantesDisponibles.Add(new CooperanteWrapper(new Cooperante { Nombre = "A", Apellido = "A1 A2" }));
            CooperantesDisponibles.Add(new CooperanteWrapper(new Cooperante { Nombre = "B", Apellido = "B1 B2" }));
            CooperantesDisponibles.Add(new CooperanteWrapper(new Cooperante { Nombre = "C", Apellido = "C1 C2" }));

            Actividad.Cooperantes.Add(new CooperanteWrapper(new Cooperante()));

            AceptarCommand = new DelegateCommand(OnAceptar, OnAceptar_CanExecute);
            NuevoCooperanteCommand = new DelegateCommand(OnNuevoCooperante);
            CancelarCommand = new DelegateCommand(OnCancelar);
            AbrirPopupCoordinadorCommand = new DelegateCommand<CooperanteWrapper>(OnAbrirPopupCoordinador);
            AbrirPopupCooperanteCommand = new DelegateCommand<CooperanteWrapper>(OnAbrirPopupCooperante);
            AceptarNuevoCooperanteCommand = new DelegateCommand(OnNuevoCooperanteCommand);
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
            if (cooperantePreviamenteSeleccionado != null)
            {
                CooperantePreviamenteSeleccionado = cooperantePreviamenteSeleccionado;
            }
        }

        private void OnNuevoCooperanteCommand()
        {
            if (_modo == "Coordinador")
            {
                if (Actividad.Coordinador != null)
                {
                    CooperantesDisponibles.Add(Actividad.Coordinador);
                }

                Actividad.Coordinador = CooperanteSeleccionado;
                Actividad.Model.EstablecerCoordinador(CooperanteSeleccionado.Model);
                // TODO --> Meterlo en el SearchBox
                CooperantesDisponibles.Remove(CooperanteSeleccionado);
            }
            else if (_modo == "Cooperante")
            {
                if (CooperantePreviamenteSeleccionado != null)
                {
                    CooperantesDisponibles.Add(CooperantePreviamenteSeleccionado);
                    Actividad.Cooperantes.Insert(
                        Actividad.Cooperantes.IndexOf(CooperantePreviamenteSeleccionado), 
                        CooperanteSeleccionado);
                    Actividad.Cooperantes.Remove(CooperantePreviamenteSeleccionado);
                }
                else 
                {
                    Actividad.Cooperantes.Insert(Actividad.Cooperantes.Count - 1, CooperanteSeleccionado);
                }

                Actividad.Model.AddCooperante(CooperanteSeleccionado.Model);
                //Actividad.Cooperantes.Add(new CooperanteWrapper(new Cooperante()));
                // TODO --> Meterlo en el SearchBox
                CooperantesDisponibles.Remove(CooperanteSeleccionado);
            }

            CooperantePreviamenteSeleccionado = null;
            PopupEstaAbierto = false;
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
