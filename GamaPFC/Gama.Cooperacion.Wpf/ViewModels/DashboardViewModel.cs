using Core;
using Gama.Cooperacion.Business;
using Gama.Cooperacion.Wpf.Eventos;
using Gama.Cooperacion.Wpf.Services;
using LiveCharts;
using LiveCharts.Wpf;
using NHibernate;
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
    public class DashboardViewModel : ViewModelBase
    {
        private IActividadRepository _actividadRepository;
        private IEventAggregator _eventAggregator;
        private ICooperanteRepository _cooperanteRepository;
        private ICooperacionSettings _settings;
        private int _mesInicial;
        private string[] _Labels;

        public DashboardViewModel(
            IActividadRepository actividadRepository,
            ICooperanteRepository cooperanteRepository,
            IEventAggregator eventAggregator, 
            ICooperacionSettings settings,
            ISession session)
        {
            _actividadRepository = actividadRepository;
            _cooperanteRepository = cooperanteRepository;
            _actividadRepository.Session = session;
            _cooperanteRepository.Session = session;
            _eventAggregator = eventAggregator;
            _settings = settings;

            UltimasActividades = new ObservableCollection<Actividad>(
                _actividadRepository.GetAll()
                .OrderBy(a => a.FechaDeFin)
                .Take(_settings.DashboardActividadesAMostrar)
                .ToArray());

            UltimosCooperantes = new ObservableCollection<Cooperante>(
                _cooperanteRepository.GetAll()
                .OrderBy(c => c.Id)
                .Take(_settings.DashboardCooperantesAMostrar)
                .ToArray());

            ActividadesNuevasPorMes = new SeriesCollection
            {
                new LineSeries
                {
                   Title = "Actividades nuevas por mes",
                   Values = new ChartValues<int> { 3, 0, 7, 4, 4 },
                }
            };

            CooperantesNuevosPorMes = new SeriesCollection
            {
                new LineSeries
                {
                   Title = "Cooperantes nuevos por mes",
                   Values = new ChartValues<int> { 4, 2, 6, 2, 4 },
                }
            };

            IncidenciasNuevasPorMes = new SeriesCollection
            {
                new LineSeries
                {
                   Title = "Incidencias nuevas por mes".ToUpper(),
                   Values = new ChartValues<int> { 2, 4, 1, 2, 1 },
                }
            };

            _mesInicial = DateTime.Today.Month - 1;

            _Labels = new[] {
                "Sep","Oct", "Nov", "Dic",
                "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic", };

            _eventAggregator.GetEvent<NuevaActividadEvent>().Subscribe(OnNuevaActividadEvent);
            _eventAggregator.GetEvent<ActividadActualizadaEvent>().Subscribe(OnActividadActualizadaEvent);

            SelectActividadCommand = new DelegateCommand<Actividad>(OnSelectActividadCommand);
            SelectCooperanteCommand = new DelegateCommand<Cooperante>(OnSelectCooperanteCommand);
        }

        public ObservableCollection<Actividad> UltimasActividades { get; private set; }
        public ObservableCollection<Cooperante> UltimosCooperantes { get; private set; }

        public SeriesCollection ActividadesNuevasPorMes { get; private set; }
        public SeriesCollection CooperantesNuevosPorMes { get; private set; }
        public SeriesCollection IncidenciasNuevasPorMes { get; private set; }
        public string[] Labels => _Labels.Skip(_mesInicial).Take(5).ToArray();

        public ICommand SelectActividadCommand { get; set; }
        public ICommand SelectCooperanteCommand { get; set; }

        private void OnSelectCooperanteCommand(Cooperante obj)
        {
            throw new NotImplementedException();
        }

        private void OnSelectActividadCommand(Actividad actividad)
        {
            _eventAggregator.GetEvent<ActividadSeleccionadaEvent>().Publish(actividad.Id);
        }

        private void OnNuevaActividadEvent(int id)
        {
            UltimasActividades.Insert(0, _actividadRepository.GetById(id));
        }

        private void OnActividadActualizadaEvent(int id)
        {
            var actividadActualizada = _actividadRepository.GetById(id);
            var indice = UltimasActividades.IndexOf(UltimasActividades.Single(a => a.Id == id));
            UltimasActividades[indice] = actividadActualizada;
        }
    }
}
