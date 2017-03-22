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
using Prism.Regions;
using Gama.Atenciones.Wpf.UIEvents;
using Gama.Atenciones.Wpf.Converters;
using Gama.Atenciones.Business;

namespace Gama.Atenciones.Wpf.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {
        private IAtencionRepository _AtencionRepository;
        private ICitaRepository _CitaRepository;
        private IEventAggregator _EventAggregator;
        private IPersonaRepository _PersonaRepository;
        private PreferenciasDeAtenciones _Settings;
        private int _MesInicialPersonas;
        private string[] _Labels;
        private int _MesInicialAtenciones;
        private string[] _LabelsTotales;

        public DashboardViewModel(IPersonaRepository personaRepository,
            ICitaRepository citaRepository,
            IAtencionRepository atencionRepository,
            IEventAggregator eventAggregator,
            PreferenciasDeAtenciones settings,
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
                    DisplayMember2 = a.Nif,
                    IconSource = a.AvatarPath,
                    Imagen = a.Imagen
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
                         a.Seguimiento, _Settings.DashboardLongitudDeSeguimientos),
                     IconSource = @"atencion_icon.png",
                     Imagen = BinaryImageConverter.GetBitmapImageFromUriSource(
                         new Uri("pack://application:,,,/Gama.Atenciones.Wpf;component/Resources/Images/atencion_icon.png")),
                 }));

            ProximasCitas = new ObservableCollection<LookupItem>(
                _CitaRepository.GetAll()
                 .OrderBy(c => c.Fecha)
                 .Where(c => c.Fecha >= DateTime.Now.Date)
                 .Take(_Settings.DashboardUltimasCitas)
                 .Select(c => new LookupItem
                 {
                     Id = c.Id,
                     DisplayMember1 = c.Fecha.ToString(),
                     DisplayMember2 = c.Sala,
                     IconSource = c.Persona.AvatarPath,
                     Imagen = c.Persona.Imagen
                 }));

            InicializarGraficos();

            SelectPersonaCommand = new DelegateCommand<LookupItem>(OnSelectPersonaCommandExecute);
            SelectCitaCommand = new DelegateCommand<LookupItem>(OnSelectCitaCommandExecute);
            SelectAtencionCommand = new DelegateCommand<LookupItem>(OnSelectAtencionCommandExecute);

            _EventAggregator.GetEvent<PersonaCreadaEvent>().Subscribe(OnPersonaCreadaEvent);
            _EventAggregator.GetEvent<AtencionCreadaEvent>().Subscribe(OnAtencionCreadaEvent);
            _EventAggregator.GetEvent<CitaCreadaEvent>().Subscribe(OnNuevaCitaEvent);
            _EventAggregator.GetEvent<PersonaEliminadaEvent>().Subscribe(OnPersonaEliminadaEvent);

            _EventAggregator.GetEvent<PersonaActualizadaEvent>().Subscribe(OnPersonaActualizadaEvent);
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

        private void OnSelectAtencionCommandExecute(LookupItem atencionLookupItem)
        {
            var atencion = _AtencionRepository.GetById(atencionLookupItem.Id);

            _EventAggregator.GetEvent<AtencionSeleccionadaEvent>().Publish(
                    new IdentificadorDeModelosPayload
                    {
                        PersonaId = atencion.Cita.Persona.Id,
                        CitaId = atencion.Cita.Id,
                        AtencionId = atencion.Id
                    }
                );
        }

        private void OnSelectCitaCommandExecute(LookupItem citaLookupItem)
        {
            var cita = _CitaRepository.GetById(citaLookupItem.Id);
            _EventAggregator.GetEvent<CitaSeleccionadaEvent>().Publish(cita.Persona.Id);
        }

        private void OnSelectPersonaCommandExecute(LookupItem persona)
        {
            _EventAggregator.GetEvent<PersonaSeleccionadaEvent>().Publish(persona.Id);
        }

        private void OnPersonaCreadaEvent(int id)
        {
            var persona = _PersonaRepository.GetById(id);
            var lookupItem = new LookupItem
            {
                Id = persona.Id,
                DisplayMember1 = persona.Nombre,
                DisplayMember2 = persona.Nif,
                IconSource = persona.AvatarPath,
                Imagen = persona.Imagen
            };

            UltimasPersonas.Insert(0, lookupItem);
        }

        private void OnPersonaEliminadaEvent(int id)
        {
            //
            // Últimas personas
            //
            UltimasPersonas.Remove(UltimasPersonas.First(x => x.Id == id));

            //
            // Últimas atenciones y próximas citas
            //
            Persona persona = _PersonaRepository.GetById(id);
            var atencionesIds = new List<int>();
            var citasIds = new List<int>();

            // Recogemos todos los ids de las atenciones para posteriormente
            // borrarlas
            foreach (var cita in persona.Citas)
            {
                citasIds.Add(cita.Id);
                if (cita.Atencion != null)
                {
                    atencionesIds.Add(cita.Atencion.Id);
                }
            }
            
            for (int i = 0; i < UltimasAtenciones.Count; i++)
            {
                if (atencionesIds.Contains(UltimasAtenciones[i].Id))
                {
                    UltimasAtenciones.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < ProximasCitas.Count; i++)
            {
                if (citasIds.Contains(ProximasCitas[i].Id))
                {
                    ProximasCitas.RemoveAt(i);
                    i--;
                }
            }



            //
            // Próximas citas
            //

            // TODO: Quitar las atenciones de últimas atenciones
            // Quitar las citas de últimas citas
        }

        private void OnAtencionCreadaEvent(int id)
        {
            var atencion = _AtencionRepository.GetById(id);
            var lookupItem = new LookupItem
            {
                Id = atencion.Id,
                DisplayMember1 = atencion.Fecha.ToString(),
                DisplayMember2 = LookupItem.ShortenStringForDisplay(
                         atencion.Seguimiento, _Settings.DashboardLongitudDeSeguimientos),
                IconSource = @"atencion_icon.png",
                Imagen = BinaryImageConverter.GetBitmapImageFromUriSource(
                         new Uri("pack://application:,,,/Gama.Atenciones.Wpf;component/Resources/Images/atencion_icon.png")),

                // TODO Poner imagen desde recursos y tal
            };
            UltimasAtenciones.Insert(0, lookupItem);
        }

        private void OnNuevaCitaEvent(int id)
        {
            var cita = _CitaRepository.GetById(id);
            var persona = _PersonaRepository.GetById(cita.Persona.Id);

            var lookupItem = new LookupItem
            {
                Id = cita.Id,
                DisplayMember1 = cita.Fecha.ToString(),
                DisplayMember2 = cita.Sala,
                //Imagen = cita.Persona.Imagen
                Imagen = persona.Imagen
            };

            if (ProximasCitas.Count > 0)
            {
                var last = DateTime.Parse(ProximasCitas.Last().DisplayMember1);
                if (cita.Fecha < last) // es antes
                {
                    int index = 0;
                    foreach (var lookup in ProximasCitas)
                    {
                        var next = DateTime.Parse(lookup.DisplayMember1);
                        if (cita.Fecha < next)
                        {
                            //ProximasCitas.Insert(index, lookupItem);
                            ProximasCitas.Add(lookupItem);
                            break;
                        }

                        index++;
                    }
                }
                else
                {
                    ProximasCitas.Add(lookupItem);
                }
            }
            else
            {
                ProximasCitas.Add(lookupItem);
            }

            OnPropertyChanged(nameof(ProximasCitas));
        }

        private void OnPersonaActualizadaEvent(int id)
        {
            var persona = _PersonaRepository.GetById(id);
            //_PersonaRepository.Session.Evict(persona);

            var personaDesactualizada = UltimasPersonas.Where(x => x.Id == id).FirstOrDefault();
            if (personaDesactualizada != null)
            {
                personaDesactualizada.DisplayMember1 = persona.Nombre;
                personaDesactualizada.DisplayMember2 = persona.Nif;
                personaDesactualizada.IconSource = persona.AvatarPath;
                personaDesactualizada.Imagen = persona.Imagen;
            }

            var citasDesactualizadas = ProximasCitas.Where(x => persona.Citas.Any(c => c.Id == x.Id)).ToList();

            foreach (var citaDesactualizada in citasDesactualizadas)
            {
                citaDesactualizada.IconSource = persona.AvatarPath;
                citaDesactualizada.Imagen = persona.Imagen;
            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            _EventAggregator.GetEvent<ActiveViewChanged>().Publish("DashboardView");
        }
    }
}
