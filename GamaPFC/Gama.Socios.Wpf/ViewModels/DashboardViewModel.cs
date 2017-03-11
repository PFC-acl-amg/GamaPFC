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
                    .Take(_Settings.DashboardUltimosSocios));

            SociosCumpliendoBirthdays = new ObservableCollection<Socio>(
                _Socios.Where(x => x.IsBirthday()));

            SociosMorosos = new ObservableCollection<Socio>(
                _Socios.Where(x => x.EsMoroso()));

            _EventAggregator.GetEvent<SocioCreadoEvent>().Subscribe(OnSocioCreadoEvent);
            _EventAggregator.GetEvent<SocioDadoDeBajaEvent>().Subscribe(OnSocioDadoDeBajaEvent);
            _EventAggregator.GetEvent<SocioActualizadoEvent>().Subscribe(OnSocioActualizadoEvent);

            SeleccionarSocioCommand = new DelegateCommand<Socio>(OnSeleccionarSocioCommandExecute);

            InicializarGraficos();
        }

        private void InicializarGraficos()
        {
            _Labels = new[] {
                "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago","Sep","Oct", "Nov", "Dic",
                "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic", };

            _MesInicialSocios = 12 + (DateTime.Now.Month - 1) - _Settings.DashboardMesesAMostrarDeSociosNuevos + 1;

            SociosNuevosPorMes = new ChartValues<int>(_SocioRepository.GetSociosNuevosPorMes(
                       _Settings.DashboardMesesAMostrarDeSociosNuevos));
        }

        public ObservableCollection<Socio> UltimosSocios { get; private set; }
        public ObservableCollection<Socio> SociosCumpliendoBirthdays { get; private set; }
        public ObservableCollection<Socio> SociosMorosos { get; private set; }
        public ChartValues<int> SociosNuevosPorMes { get; set;}
        public ICommand SeleccionarSocioCommand { get; private set; }

        public string[] SociosLabels =>
            _Labels.Skip(_MesInicialSocios)
                .Take(_Settings.DashboardMesesAMostrarDeSociosNuevos).ToArray();

        private void OnSeleccionarSocioCommandExecute(Socio socio)
        {
            _EventAggregator.GetEvent<SocioSeleccionadoEvent>().Publish(socio.Id);
        }

        private void OnSocioCreadoEvent(int id)
        {
            var socio = _SocioRepository.GetById(id);

            UltimosSocios.Insert(0, socio);

            // Si es su cumpleaños, añadirllo
            if (socio.IsBirthday())
            {
                SociosCumpliendoBirthdays.Insert(0, socio);
            }
        }

        private void OnSocioDadoDeBajaEvent(int id)
        {
            UltimosSocios.Remove(UltimosSocios.First(x => x.Id == id));
            SociosCumpliendoBirthdays.Remove(SociosCumpliendoBirthdays.First(x => x.Id == id));
            
            // No lo quitamos de la lista de morosos ya que aunque esté dado de baja, queremos
            // que se visualice
        }

        private void OnSocioActualizadoEvent(int id)
        {
            var socio = _SocioRepository.GetById(id);

            var socioDesactualizado = UltimosSocios.Where(x => x.Id == id).FirstOrDefault();
            if (socioDesactualizado != null)
            {
                socioDesactualizado.Nombre = socio.Nombre;
                socioDesactualizado.Nif = socio.Nif;
                socioDesactualizado.AvatarPath = socio.AvatarPath;
            }

            // Actualizar a cumpleañeros o lista de morosos. Tal vez hay que quitarlo o ponerlo.

            // Si es cumpleañeros y no está en la lista, añadirlo
            if (socioDesactualizado.IsBirthday() &&
                SociosCumpliendoBirthdays.FirstOrDefault(x => x.Id == id) == null)
            {
                SociosCumpliendoBirthdays.Insert(0, socioDesactualizado);
            }

            // Si ahora no es cumpleañero y está en la lista, quitarlo
            if (!socioDesactualizado.IsBirthday() &&
                SociosCumpliendoBirthdays.FirstOrDefault(x => x.Id == id) != null)
            {
                SociosCumpliendoBirthdays.Remove(SociosCumpliendoBirthdays.FirstOrDefault(x => x.Id == id));
            }

            // Si es moroso y no está en la lista, añadirlo
            if (socioDesactualizado.EsMoroso() &&
                SociosMorosos.FirstOrDefault(x => x.Id == id) == null)
            {
                SociosMorosos.Insert(0, socioDesactualizado);
            }

            // Si ahora no es moroso y está en la lista, quitarlo
            if (!socioDesactualizado.EsMoroso() &&
                SociosMorosos.FirstOrDefault(x => x.Id == id) != null)
            {
                SociosMorosos.Remove(SociosMorosos.FirstOrDefault(x => x.Id == id));
            }
        }
    }
}
