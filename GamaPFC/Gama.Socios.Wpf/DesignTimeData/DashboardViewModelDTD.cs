using Gama.Common.CustomControls;
using Gama.Socios.Business;
using Gama.Socios.Wpf.FakeServices;
using Gama.Socios.Wpf.Services;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Socios.Wpf.DesignTimeData
{
    public class DashboardViewModelDTD
    {
        private FakeSocioRepository _SocioRepository;
        private PreferenciasDeSocios _Settings;
        private ObservableCollection<Socio> _Socios;
        private string[] _Labels;
        private int _MesInicialSocios;

        public DashboardViewModelDTD()
        {
            _Settings = new PreferenciasDeSocios();
            _SocioRepository = new FakeSocioRepository();

            _Socios = new ObservableCollection<Socio>(_SocioRepository.GetAll());

            UltimosSocios = new ObservableCollection<LookupItem>(
                    _Socios
                    .OrderBy(x => x.Id)
                    .Take(_Settings.DashboardMesesAMostrarDeSociosNuevos)
                    .Select(a => new LookupItem
                    {
                        Id = a.Id,
                        DisplayMember1 = a.Nombre,
                        DisplayMember2 = a.Nif,
                    }));

            SociosCumpliendoBirthdays = new ObservableCollection<LookupItem>(
                _Socios.Select(a => new LookupItem
                {
                    Id = a.Id,
                    DisplayMember1 = a.Nombre,
                    DisplayMember2 = a.Nif,
                }));

            SociosMorosos = new ObservableCollection<LookupItem>(
                _Socios.Select(a => new LookupItem
                {
                    Id = a.Id,
                    DisplayMember1 = a.Nombre,
                    DisplayMember2 = a.Nif,
                }));

            InicializarGraficos();
        }

        public ObservableCollection<LookupItem> UltimosSocios { get; private set; }
        public ObservableCollection<LookupItem> SociosCumpliendoBirthdays { get; private set; }
        public ObservableCollection<LookupItem> SociosMorosos { get; private set; }
        public ChartValues<int> SociosNuevosPorMes { get; set; }

        private void InicializarGraficos()
        {
            _Labels = new[] {
                "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago","Sep","Oct", "Nov", "Dic",
                "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic", };

            _MesInicialSocios = 12 + (DateTime.Now.Month - 1) - _Settings.DashboardMesesAMostrarDeSociosNuevos + 1;

            SociosNuevosPorMes = new ChartValues<int>(_SocioRepository.GetSociosNuevosPorMes(
                       _Settings.DashboardMesesAMostrarDeSociosNuevos));
        }
    }
}
