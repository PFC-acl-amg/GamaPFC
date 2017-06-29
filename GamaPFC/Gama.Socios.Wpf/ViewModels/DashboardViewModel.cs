using Core;
using Gama.Common.CustomControls;
using Gama.Common.Eventos;
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
        private PreferenciasDeSocios _Settings;
        private ISocioRepository _SocioRepository;
        private ObservableCollection<Socio> _Socios;
        private string[] _Labels;
        private int _MesInicialSocios;

        public DashboardViewModel(ISocioRepository socioRepository,
            IEventAggregator eventAggregator, 
            PreferenciasDeSocios settings,
            ISession session)
        {
            _SocioRepository = socioRepository;
            _SocioRepository.Session = session;
            _EventAggregator = eventAggregator;
            _Settings = settings;

            _Socios = new ObservableCollection<Socio>(_SocioRepository.GetAll());

            UltimosSocios = new ObservableCollection<LookupItem>(
                    _Socios
                    .OrderBy(x => x.Id)
                    .Take(_Settings.DashboardUltimosSocios)
                    .Select(a => new LookupItem
                    {
                        Id = a.Id,
                        DisplayMember1 = a.Nombre,
                        DisplayMember2 = a.Nif,
                        Imagen = a.Imagen
                    }));

            SociosCumpliendoBirthdays = new ObservableCollection<LookupItem>(
                _Socios
                    .Where(x => x.IsBirthday())
                    .Take(_Settings.DashboardSociosCumpliendoBirthdays)
                    .Select(a => new LookupItem
                    {
                        Id = a.Id,
                        DisplayMember1 = a.Nombre,
                        DisplayMember2 = a.Nif,
                        Imagen = a.Imagen
                    }));

            SociosMorosos = new ObservableCollection<LookupItem>(
                _Socios
                    .Where(x => x.EsMoroso(_Settings.MesesParaSerConsideradoMoroso))
                    .Take(_Settings.DashboardSociosMorosos)
                    .Select(a => new LookupItem
                    {
                        Id = a.Id,
                        DisplayMember1 = a.Nombre,
                        DisplayMember2 = a.Nif,
                        Imagen = a.Imagen
                    }));

            _EventAggregator.GetEvent<SocioCreadoEvent>().Subscribe(OnSocioCreadoEvent);
            _EventAggregator.GetEvent<SocioDadoDeBajaEvent>().Subscribe(OnSocioDadoDeBajaEvent);
            _EventAggregator.GetEvent<SocioActualizadoEvent>().Subscribe(OnSocioActualizadoEvent);

            _EventAggregator.GetEvent<PreferenciasActualizadasEvent>().Subscribe(OnPreferenciasActualizadasEvent);

            SeleccionarSocioCommand = new DelegateCommand<LookupItem>(OnSeleccionarSocioCommandExecute);

            InicializarGraficos();
        }

        private void OnPreferenciasActualizadasEvent()
        {
            if (UltimosSocios.Count != _Settings.DashboardUltimosSocios)
            {
                UltimosSocios = new ObservableCollection<LookupItem>(
                        _Socios
                        .OrderBy(x => x.Id)
                        .Take(_Settings.DashboardUltimosSocios)
                        .Select(a => new LookupItem
                        {
                            Id = a.Id,
                            DisplayMember1 = a.Nombre,
                            DisplayMember2 = a.Nif,
                            Imagen = a.Imagen
                        }));

                OnPropertyChanged(nameof(UltimosSocios));
            }

            if (SociosCumpliendoBirthdays.Count != _Settings.DashboardSociosCumpliendoBirthdays)
            {
                SociosCumpliendoBirthdays = new ObservableCollection<LookupItem>(
                    _Socios
                        .Where(x => x.IsBirthday())
                        .Take(_Settings.DashboardSociosCumpliendoBirthdays)
                        .Select(a => new LookupItem
                        {
                            Id = a.Id,
                            DisplayMember1 = a.Nombre,
                            DisplayMember2 = a.Nif,
                            Imagen = a.Imagen
                        }));

                OnPropertyChanged(nameof(SociosCumpliendoBirthdays));
            }

            if (SociosMorosos.Count != _Settings.DashboardSociosMorosos)
            {
                SociosMorosos = new ObservableCollection<LookupItem>(
                    _Socios
                        .Where(x => x.EsMoroso(_Settings.MesesParaSerConsideradoMoroso))
                        .Take(_Settings.DashboardSociosMorosos)
                        .Select(a => new LookupItem
                        {
                            Id = a.Id,
                            DisplayMember1 = a.Nombre,
                            DisplayMember2 = a.Nif,
                            Imagen = a.Imagen
                        }));

                OnPropertyChanged(nameof(SociosMorosos));
            }
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

        public ObservableCollection<LookupItem> UltimosSocios { get; private set; }
        public ObservableCollection<LookupItem> SociosCumpliendoBirthdays { get; private set; }
        public ObservableCollection<LookupItem> SociosMorosos { get; private set; }
        public ChartValues<int> SociosNuevosPorMes { get; set;}
        public ICommand SeleccionarSocioCommand { get; private set; }

        public string[] SociosLabels =>
            _Labels.Skip(_MesInicialSocios)
                .Take(_Settings.DashboardMesesAMostrarDeSociosNuevos).ToArray();

        private void OnSeleccionarSocioCommandExecute(LookupItem socio)
        {
            _EventAggregator.GetEvent<SocioSeleccionadoEvent>().Publish(socio.Id);
        }

        private void OnSocioCreadoEvent(int id)
        {
            var socio = _SocioRepository.GetById(id);

            var lookupItem = new LookupItem
            {
                Id = socio.Id,
                DisplayMember1 = socio.Nombre,
                DisplayMember2 = socio.Nif,
                Imagen = socio.Imagen
            };

            UltimosSocios.Insert(0, lookupItem);

            // Si es su cumpleaños, añadirllo
            if (socio.IsBirthday())
            {
                SociosCumpliendoBirthdays.Insert(0, lookupItem);
            }
        }

        private void OnSocioDadoDeBajaEvent(int id)
        {
            UltimosSocios.Remove(UltimosSocios.First(x => x.Id == id));
            SociosCumpliendoBirthdays.Remove(SociosCumpliendoBirthdays.First(x => x.Id == id));
            
            // No lo quitamos de la lista de morosos ya que aunque esté dado de baja, queremos
            // que se visualice
        }

        private void OnSocioActualizadoEvent(Socio socioActualizado)
        {
            var socio = socioActualizado;
            int id = socio.Id;

            var socioDesactualizado = UltimosSocios.Where(x => x.Id == id).FirstOrDefault();
            if (socioDesactualizado != null)
            {
                socioDesactualizado.DisplayMember1 = socio.Nombre;
                socioDesactualizado.DisplayMember2 = socio.Nif;
                socioDesactualizado.Imagen = socio.Imagen;
            }

            // Actualizar a cumpleañeros o lista de morosos. Tal vez hay que quitarlo o ponerlo.

            // Si es cumpleañeros y no está en la lista, añadirlo
            if (socioActualizado.IsBirthday() &&
                SociosCumpliendoBirthdays.FirstOrDefault(x => x.Id == id) == null)
            {
                SociosCumpliendoBirthdays.Insert(0, socioDesactualizado);
            }

            // Si ahora no es cumpleañero y está en la lista, quitarlo
            if (!socioActualizado.IsBirthday() &&
                SociosCumpliendoBirthdays.FirstOrDefault(x => x.Id == id) != null)
            {
                SociosCumpliendoBirthdays.Remove(SociosCumpliendoBirthdays.FirstOrDefault(x => x.Id == id));
            }

            // Si es moroso y no está en la lista, añadirlo
            if (socioActualizado.EsMoroso() &&
                SociosMorosos.FirstOrDefault(x => x.Id == id) == null)
            {
                SociosMorosos.Insert(0, socioDesactualizado);
            }

            // Si ahora no es moroso y está en la lista, quitarlo
            if (!socioActualizado.EsMoroso(_Settings.MesesParaSerConsideradoMoroso) &&
                SociosMorosos.FirstOrDefault(x => x.Id == id) != null)
            {
                SociosMorosos.Remove(SociosMorosos.FirstOrDefault(x => x.Id == id));
            }
        }
    }
}
