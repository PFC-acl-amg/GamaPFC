using Core;
using Gama.Cooperacion.Business;
using Gama.Cooperacion.Wpf.Eventos;
using Gama.Cooperacion.Wpf.Services;
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

        public Actividad Actividad { get; set; }

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

        public ObservableCollection<Cooperante> Cooperantes { get; private set; }

        public NuevaActividadViewModel(IActividadRepository actividadRepository,
            IEventAggregator eventAggregator)
        {
            _actividadRepository = actividadRepository;
            _eventAggregator = eventAggregator;

            Actividad = new Actividad();
            Actividad.Titulo = "Nombre del proyecto...";
            Actividad.Descripcion = "Descripción del proyecto...";

            Cooperantes = new ObservableCollection<Cooperante>();
            Cooperantes.Add(new Cooperante());

            AceptarCommand = new DelegateCommand(OnAceptar, OnAceptar_CanExecute);
            NuevoCooperanteCommand = new DelegateCommand(OnNuevoCooperante);
            CancelarCommand = new DelegateCommand(OnCancelar);
        }

        public ICommand AceptarCommand { get; set; }
        public ICommand NuevoCooperanteCommand { get; set; }
        public ICommand CancelarCommand { get; private set; }

        private void OnAceptar()
        {
            _actividadRepository.Create(Actividad);
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
            Cooperantes.Add(new Cooperante());
        }

        private void OnCancelar()
        {
            Cerrar = true;
        }
    }
}
