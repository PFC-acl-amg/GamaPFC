using Core;
using Gama.Cooperacion.Wpf.Services;
using NHibernate;
using Prism.Events;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using Gama.Cooperacion.Wpf.Eventos;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gama.Cooperacion.Wpf.Wrappers;
using System.Windows.Input;
using Prism.Commands;
using Core.Util;

namespace Gama.Cooperacion.Wpf.ViewModels
{
    public class CalendarioDeActividadesViewModel : ViewModelBase
    {
        private IActividadRepository _ActividadRepository;
        private List<ActividadWrapper> _Actividades;

        public CalendarioDeActividadesViewModel(
            EventAggregator eventAggregator, 
            IActividadRepository actividadRepository,
            ISession session)
        {
            _ActividadRepository = actividadRepository;
            _ActividadRepository.Session = session;
            _EventAggregator = eventAggregator;

            _Actividades = new List<ActividadWrapper>(
                _ActividadRepository.GetAll().
                Select(x => new ActividadWrapper(x)));
            int counter = 18;
            foreach (var actividad in _Actividades)
            {
                actividad.FechaDeFin = DateTime.Now.AddHours(counter);
                counter += 18;
            }
            Actividades = new ObservableCollection<ActividadWrapper>(_Actividades);
            SeleccionarActividadCommand = new DelegateCommand<ActividadWrapper>(OnSeleccionarActividadCommandExecute);
            ResetearFechasCommand = new DelegateCommand(() =>
            {
                FechaDeInicio = null;
                FechaDeFin = null;
            });
        }

        public ObservableCollection<ActividadWrapper> Actividades { get; private set; }
        public ICommand ResetearFechasCommand { get; private set; }
        public ICommand SeleccionarActividadCommand { get; private set; }
        public bool _AplicarFiltroDeFecha;

        private DateTime? _FechaDeInicio;
        public DateTime? FechaDeInicio
        {
            get { return _FechaDeInicio; }
            set { SetProperty(ref _FechaDeInicio, value); FiltrarPorFecha(); }
        }

        private DateTime? _FechaDeFin;
        public DateTime? FechaDeFin
        {
            get { return _FechaDeFin; }
            set { SetProperty(ref _FechaDeFin, value); FiltrarPorFecha(); }
        }

        private int _Refresh;
        private EventAggregator _EventAggregator;

        public int Refresh
        {
            get { return _Refresh; }
            set { SetProperty(ref _Refresh, value); }
        }

        private void OnSeleccionarActividadCommandExecute(ActividadWrapper wrapper)
        {
            _EventAggregator.GetEvent<ActividadSeleccionadaEvent>().Publish(wrapper.Id);
        }

        // Si no, carga todos los elementos. Cada vez que hay algún cambio
        // en personas, citas o atenciones, se llamará a esta función para
        // refrescar la vista.
        public void FiltrarPorFecha()
        {
            var fechaDeInicio = FechaDeInicio ?? DateTime.Now.AddYears(-100);
            var fechaDeFin = FechaDeFin ?? DateTime.Now.AddYears(10);

            if (_AplicarFiltroDeFecha)
                Actividades =
                    new ObservableCollection<ActividadWrapper>(
                    _Actividades
                    .Where(c => c.FechaDeFin.IsBetween(fechaDeInicio, FechaDeFin))
                    .OrderBy(c => c.FechaDeFin));
            else
                Actividades =
                    new ObservableCollection<ActividadWrapper>(
                    _Actividades
                    .OrderBy(c => c.FechaDeFin));

            OnPropertyChanged(nameof(Actividades));
        }

        public override void OnActualizarServidor()
        {
            _Actividades = new List<ActividadWrapper>(
               _ActividadRepository.GetAll().
               Select(x => new ActividadWrapper(x)));
            int counter = 18;
            foreach (var actividad in _Actividades)
            {
                actividad.FechaDeFin = DateTime.Now.AddHours(counter);
                counter += 18;
            }
            Actividades = new ObservableCollection<ActividadWrapper>(_Actividades);
        }
    }
}
