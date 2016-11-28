using Core;
using Gama.Atenciones.Wpf.Eventos;
using Gama.Atenciones.Wpf.Services;
using Gama.Common.CustomControls;
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
            _AtencionRepository.Session = session;
            _PersonaRepository.Session = session;
            _CitaRepository.Session = session;
            _EventAggregator = eventAggregator;
            _Settings = settings;

            UltimasPersonas = new ObservableCollection<LookupItem>(
                _PersonaRepository.GetAll()
                    .OrderBy(p => p.Id)
                    //.OrderBy(p => p.CreatedAt)
                    //.OrderBy(p => p.UpdatedAt)
                    .Take(_Settings.DashboardUltimasPersonas)
                .Select(a => new LookupItem
                {
                    Id = a.Id,
                    DisplayMember1 = a.Nombre,
                    DisplayMember2 = a.Nif
                }));

            ProximasCitas = new ObservableCollection<LookupItem>(
                _CitaRepository.GetAll()
                 .OrderBy(c => c.Inicio)
                 .Where(c => c.Inicio >= DateTime.Now.Date)
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

            SelectPersonaCommand = new DelegateCommand<LookupItem>(OnSelectPersonaCommandExecute);
            SelectCitaCommand = new DelegateCommand<LookupItem>(OnSelectCitaCommandExecute);
            SelectAtencionCommand = new DelegateCommand<LookupItem>(OnSelectAtencionCommandExecute);

            //_EventAggregator.GetEvent<PersonaCreadaEvent>().Subscribe(OnNuevaPersonaEvent);
            _EventAggregator.GetEvent<AtencionCreadaEvent>().Subscribe(OnNuevaAtencionEvent);
            _EventAggregator.GetEvent<CitaCreadaEvent>().Subscribe(OnNuevaCitaEvent);
        }

        public ObservableCollection<LookupItem> UltimasAtenciones { get; private set; }
        public ObservableCollection<LookupItem> ProximasCitas { get; private set; }
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

        public ICommand SelectPersonaCommand { get; private set; }
        public ICommand SelectCitaCommand { get; private set; }
        public ICommand SelectAtencionCommand { get; private set; }

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

        private void OnSelectAtencionCommandExecute(LookupItem atencion)
        {
            _EventAggregator.GetEvent<AtencionSeleccionadaEvent>().Publish(atencion.Id);
        }

        private void OnSelectCitaCommandExecute(LookupItem cita)
        {
            _EventAggregator.GetEvent<CitaSeleccionadaEvent>().Publish(cita.Id);
        }

        private void OnSelectPersonaCommandExecute(LookupItem persona)
        {
            _EventAggregator.GetEvent<PersonaSeleccionadaEvent>().Publish(persona.Id);
        }

        private void OnNuevaPersonaEvent(int id)
        {
            var persona = _PersonaRepository.GetById(id);
            var lookupItem = new LookupItem
            {
                Id = persona.Id,
                DisplayMember1 = persona.Nombre,
                DisplayMember2 = persona.Nif
            };

            UltimasPersonas.Insert(0, lookupItem);
        }

        private void OnNuevaAtencionEvent(int id)
        {
            var atencion = _AtencionRepository.GetById(id);
            var lookupItem = new LookupItem
            {
                Id = atencion.Id,
                DisplayMember1 = atencion.Fecha.ToString(),
                DisplayMember2 = LookupItem.ShortenStringForDisplay(
                         atencion.Seguimiento, _Settings.DashboardLongitudDeSeguimientos)
            };
            UltimasAtenciones.Insert(0, lookupItem);
        }

        private void OnNuevaCitaEvent(int id)
        {
            var cita = _CitaRepository.GetById(id);
            var lookupItem = new LookupItem
            {
                Id = cita.Id,
                DisplayMember1 = cita.Inicio.ToString(),
                DisplayMember2 = cita.Sala
            };

            if (ProximasCitas.Count > 0)
            {

                var last = DateTime.Parse(ProximasCitas.Last().DisplayMember1);
                if (cita.Inicio < last) // es antes
                {
                    int index = 0;
                    foreach (var lookup in ProximasCitas)
                    {
                        var next = DateTime.Parse(lookup.DisplayMember1);
                        if (cita.Inicio < next)
                        {
                            //ProximasCitas.Insert(index, lookupItem);
                            ProximasCitas.Add(lookupItem);
                            break;
                        }

                        index++;
                    }
                }
            }
            else
            {
                ProximasCitas.Add(lookupItem);
            }
        }
    }
}
