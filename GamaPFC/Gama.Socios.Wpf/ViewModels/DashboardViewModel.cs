using Core;
using Gama.Socios.Business;
using Gama.Socios.Wpf.Eventos;
using Gama.Socios.Wpf.Services;
using LiveCharts;
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

namespace Gama.Socios.Wpf.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {
        private IEventAggregator _EventAggregator;
        private ISociosSettings _Settings;
        private ISocioRepository _SocioRepository;
        private ObservableCollection<Socio> _Socios;
        private string[] _Labels;
        private int _MesInicialSocios;

        public DashboardViewModel(ISocioRepository socioRepository,
            IEventAggregator eventAggregator, 
            ISociosSettings settings,
            ISession session)
        {
            _SocioRepository = socioRepository;
            _SocioRepository.Session = session;
            _EventAggregator = eventAggregator;
            _Settings = settings;

            _Socios = new ObservableCollection<Socio>(_SocioRepository.GetAll());

            UltimosSocios = new ObservableCollection<Socio>(
                    _Socios
                    .OrderBy(x => x.Id)
                    .Take(_Settings.DashboardMesesAMostrarDeSociosNuevas));

            SociosCumpliendoBirthdays = new ObservableCollection<Socio>(
                _Socios.Where(x => x.IsBirthday()));

            SociosMorosos = new ObservableCollection<Socio>(
                _Socios.Where(x => x.EsMoroso()));

            _EventAggregator.GetEvent<SocioCreadoEvent>().Subscribe(OnSocioCreadoEvent);
            _EventAggregator.GetEvent<SocioEliminadoEvent>().Subscribe(OnSocioEliminadoEvent);

            SeleccionarSocioCommand = new DelegateCommand<Socio>(OnSeleccionarSocioCommandExecute);

            InicializarGraficos();
        }

        private void InicializarGraficos()
        {
            _Labels = new[] {
                "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago","Sep","Oct", "Nov", "Dic",
                "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic", };

            _MesInicialSocios = 12 + (DateTime.Now.Month - 1) - _Settings.DashboardMesesAMostrarDeSociosNuevas + 1;

            SociosNuevosPorMes = new ChartValues<int>(_SocioRepository.GetSociosNuevosPorMes(
                       _Settings.DashboardMesesAMostrarDeSociosNuevas));
        }

        public ObservableCollection<Socio> UltimosSocios { get; private set; }
        public ObservableCollection<Socio> SociosCumpliendoBirthdays { get; private set; }
        public ObservableCollection<Socio> SociosMorosos { get; private set; }
        public ChartValues<int> SociosNuevosPorMes { get; set;}
        public ICommand SeleccionarSocioCommand { get; private set; }

        public string[] SociosLabels =>
            _Labels.Skip(_MesInicialSocios)
                .Take(_Settings.DashboardMesesAMostrarDeSociosNuevas).ToArray();

        private void OnSeleccionarSocioCommandExecute(Socio socio)
        {
            _EventAggregator.GetEvent<SocioSeleccionadoEvent>().Publish(socio.Id);
        }

        private void OnSocioCreadoEvent(int id)
        {
            var socio = _SocioRepository.GetById(id);

            UltimosSocios.Insert(0, socio);
        }

        private void OnSocioEliminadoEvent(int id)
        {
            throw new NotImplementedException();
        }
    }
}
