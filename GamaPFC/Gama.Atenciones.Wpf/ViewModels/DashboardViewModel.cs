using Core;
using Gama.Atenciones.Wpf.Services;
using Gama.Common.CustomControls;
using LiveCharts;
using NHibernate;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Atenciones.Wpf.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {
        private IAtencionRepository _AtencionRepository;
        private ICitaRepository _CitaRepository;
        private IEventAggregator _EventAggregator;
        private IPersonaRepository _PersonaRepository;
        private IAtencionesSettings _Settings;
        private int _MesInicialPersonas;
        private string[] _Labels;
        private int _MesInicialAtenciones;
        private string[] _LabelsTotales;

        public DashboardViewModel(IPersonaRepository personaRepository,
            ICitaRepository citaRepository,
            IAtencionRepository atencionRepository,
            IEventAggregator eventAggregator,
            IAtencionesSettings settings,
            ISession session)
        {
            _PersonaRepository = personaRepository;
            _CitaRepository = citaRepository;
            _AtencionRepository = atencionRepository;
            _PersonaRepository.Session = session;
            _CitaRepository.Session = session;
            _EventAggregator = eventAggregator;
            _Settings = settings;

            UltimasPersonas = new ObservableCollection<LookupItem>(
                _PersonaRepository.GetAll()
                    .OrderBy(p => p.CreatedAt)
                    .OrderBy(p => p.UpdatedAt)
                    .Take(_Settings.DashboardUltimasPersonas)
                .Select(a => new LookupItem
                {
                    Id = a.Id,
                    DisplayMember1 = a.Nombre,
                    DisplayMember2 = a.Nif
                }));

            UltimasCitas = new ObservableCollection<LookupItem>(
                _CitaRepository.GetAll()
                 .OrderBy(c => c.Inicio)
                 .Take(_Settings.DashboardUltimasCitas)
                 .Select(c => new LookupItem
                 {
                     Id = c.Id,
                     DisplayMember1 = c.Inicio.ToString(),
                     DisplayMember2 = c.Sala
                 }));

            UltimasAtenciones = new ObservableCollection<LookupItem>(
                _AtencionRepository.GetAll()
                 .OrderBy(a => a.Fecha)
                 .Take(_Settings.DashboardUltimasAtenciones)
                 .Select(a => new LookupItem
                 {
                     Id = a.Id,
                     DisplayMember1 = a.Fecha.ToString(),
                     DisplayMember2 = LookupItem.ShortenStringForDisplay(
                         a.Seguimiento, _Settings.DashboardLongitudDeSeguimientos)
                 }));

            InicializarGraficos();
        }

        public ObservableCollection<LookupItem> UltimasAtenciones { get; private set; }
        public ObservableCollection<LookupItem> UltimasCitas { get; private set; }
        public ObservableCollection<LookupItem> UltimasPersonas { get; private set; }
        public ChartValues<int> PersonasNuevasPorMes { get; private set; }
        public ChartValues<int> AtencionesNuevasPorMes { get; private set; }
        public ChartValues<int> Totales { get; private set; }

        public string[] PersonasLabels =>
            _Labels.Skip(_MesInicialPersonas)
                .Take(_Settings.DashboardMesesAMostrarDePersonasNuevas).ToArray();

        public string[] AtencionesLabels =>
            _Labels.Skip(_MesInicialAtenciones)
                .Take(_Settings.DashboardMesesAMostrarDeAtencionesNuevas).ToArray();

        public string[] TotalesLabels => _LabelsTotales;

        private void InicializarGraficos()
        {
            _Labels = new[] {
                "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago","Sep","Oct", "Nov", "Dic",
                "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic", };
            _LabelsTotales = new[] { "Personas", "Citas", "Atenciones" };

            _MesInicialPersonas = 12 + (DateTime.Now.Month - 1) - _Settings.DashboardMesesAMostrarDePersonasNuevas + 1;
            _MesInicialAtenciones = 12 + (DateTime.Now.Month - 1) - _Settings.DashboardMesesAMostrarDeAtencionesNuevas + 1;

            PersonasNuevasPorMes = new ChartValues<int>(_PersonaRepository.GetPersonasNuevasPorMes(
                       _Settings.DashboardMesesAMostrarDePersonasNuevas));

            AtencionesNuevasPorMes = new ChartValues<int>(_AtencionRepository.GetAtencionesNuevasPorMes(
                       _Settings.DashboardMesesAMostrarDeAtencionesNuevas));

            Totales = new ChartValues<int>(
                new int[] {
                    _PersonaRepository.CountAll(),
                    _CitaRepository.CountAll(),
                    _AtencionRepository.CountAll()
                });
        }
    }
}
