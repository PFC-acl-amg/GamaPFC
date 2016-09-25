using Gama.Common.CustomControls;
using Gama.Cooperacion.Business;
using Gama.Cooperacion.Wpf.Services;
using Gama.Cooperacion.Wpf.ViewModels;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.DesignTimeData
{
    public class DashboardViewModelDTD 
    {
        private CooperacionSettings _settings;
        private string[] _Labels;
        private int _mesInicial;

        public DashboardViewModelDTD()
        {
            _settings = new CooperacionSettings();

            UltimasActividades = new ObservableCollection<LookupItem>(
                new FakeActividadRepository().GetAll()
                .OrderBy(a => a.FechaDeFin)
                .Take(_settings.DashboardActividadesAMostrar)
                .Select(a => new LookupItem
                {
                    DisplayMember1 = LookupItem.ShortenStringForDisplay(a.Titulo,
                        _settings.DashboardActividadesLongitudDeTitulos),
                }));

            UltimosCooperantes = new ObservableCollection<Cooperante>(
                new FakeCooperanteRepository().GetAll()
                .OrderBy(c => c.Id)
                .Take(_settings.DashboardCooperantesAMostrar)
                .ToArray());

            ActividadesNuevasPorMes = new ChartValues<int> { 3, 0, 7, 4, 4 };

            CooperantesNuevosPorMes = new SeriesCollection
            {
                new LineSeries
                {
                   Title = "Cooperantes nuevos por mes".ToUpper(),
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

        }

        public ObservableCollection<LookupItem> UltimasActividades { get; private set; }
        public ObservableCollection<Cooperante> UltimosCooperantes { get; private set; }
        public ChartValues<int> ActividadesNuevasPorMes { get; private set; }
        public SeriesCollection CooperantesNuevosPorMes { get; private set; }
        public SeriesCollection IncidenciasNuevasPorMes { get; private set; }

        public string[] Labels => _Labels.Skip(_mesInicial).Take(5).ToArray();

    }
}
