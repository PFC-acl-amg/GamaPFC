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
        private SociosSettings _Settings;
        private ObservableCollection<Socio> _Socios;
        private string[] _Labels;
        private int _MesInicialSocios;

        public DashboardViewModelDTD()
        {
            _Settings = new SociosSettings();
            _SocioRepository = new FakeSocioRepository();

            _Socios = new ObservableCollection<Socio>(_SocioRepository.GetAll());

            UltimosSocios = new ObservableCollection<Socio>(
                    _Socios
                    .OrderBy(x => x.Id)
                    .Take(_Settings.DashboardMesesAMostrarDeSociosNuevas));

            SociosCumpliendoBirthdays = new ObservableCollection<Socio>(
                _Socios.Where(x => x.IsBirthday()));

            SociosMorosos = new ObservableCollection<Socio>(
                _Socios.Where(x => x.EsMoroso()));

            InicializarGraficos();
        }

        public ObservableCollection<Socio> UltimosSocios { get; private set; }
        public ObservableCollection<Socio> SociosCumpliendoBirthdays { get; private set; }
        public ObservableCollection<Socio> SociosMorosos { get; private set; }
        public ChartValues<int> SociosNuevosPorMes { get; set; }

        private void InicializarGraficos()
        {
            _Labels = new[] {
                "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago","Sep","Oct", "Nov", "Dic",
                "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic", };

            _MesInicialSocios = 12 + (DateTime.Now.Month - 1) - _Settings.DashboardMesesAMostrarDeSociosNuevas + 1;

            SociosNuevosPorMes = new ChartValues<int>(_SocioRepository.GetSociosNuevosPorMes(
                       _Settings.DashboardMesesAMostrarDeSociosNuevas));
        }
    }
}
