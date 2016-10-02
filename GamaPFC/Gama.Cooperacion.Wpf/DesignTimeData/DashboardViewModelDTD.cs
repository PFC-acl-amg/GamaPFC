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
        private int _mesInicialActividades;
        private int _mesInicialCooperantes;

        public DashboardViewModelDTD()
        {
            _settings = new CooperacionSettings();

            UltimasActividades = new ObservableCollection<LookupItem>(
                new FakeActividadRepository().GetAll()
                .OrderBy(a => a.FechaDeFin)
                .Take(_settings.DashboardActividadesAMostrar)
                .Select(a => new LookupItem
                {
                    Id = a.Id,
                    DisplayMember1 = LookupItem.ShortenStringForDisplay(a.Titulo,
                        _settings.DashboardActividadesLongitudDeTitulos),
                }));

            UltimosCooperantes = new ObservableCollection<Cooperante>(
                new FakeCooperanteRepository().GetAll()
                .OrderBy(c => c.Id)
                .Take(_settings.DashboardCooperantesAMostrar)
                .ToArray());

            ActividadesNuevasPorMes = new ChartValues<int> { 3, 0, 7, 4, 4 };

            CooperantesNuevosPorMes = new ChartValues<int> { 3, 0, 7, 4, 4 };

            IncidenciasNuevasPorMes = new ChartValues<int> { 3, 0, 7, 4, 4 };

            _mesInicialActividades = 12 + (DateTime.Now.Month - 1) - _settings.DashboardMesesAMostrarDeActividadesNuevas + 1;
            _mesInicialCooperantes = 12 + (DateTime.Now.Month - 1) - _settings.DashboardMesesAMostrarDeCooperantesNuevos + 1;

            _Labels = new[] {
                "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago","Sep","Oct", "Nov", "Dic",
                "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic", };

        }

        public ObservableCollection<LookupItem> UltimasActividades { get; private set; }
        public ObservableCollection<Cooperante> UltimosCooperantes { get; private set; }
        public ChartValues<int> ActividadesNuevasPorMes { get; private set; }
        public ChartValues<int> CooperantesNuevosPorMes { get; private set; }
        public ChartValues<int> IncidenciasNuevasPorMes { get; private set; }

        public string[] ActividadesLabels =>
            _Labels.Skip(_mesInicialActividades)
                .Take(_settings.DashboardMesesAMostrarDeActividadesNuevas).ToArray();

        public string[] CooperantesLabels => 
            _Labels.Skip(_mesInicialCooperantes)
                .Take(_settings.DashboardMesesAMostrarDeCooperantesNuevos).ToArray();

    }
}
