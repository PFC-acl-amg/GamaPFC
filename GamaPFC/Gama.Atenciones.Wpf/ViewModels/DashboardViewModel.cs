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
using System.ComponentModel;

namespace Gama.Atenciones.Wpf.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {
        private IAtencionRepository _AtencionRepository;
        private ICitaRepository _CitaRepository;
        private IEventAggregator _EventAggregator;
        private IPersonaRepository _PersonaRepository;
        private PreferenciasDeAtenciones _Settings;
        private List<Persona> _Personas;
        private List<Atencion> _Atenciones;
        private List<Cita> _Citas;
        private bool _FiltradoEstaActivo = false;

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

            _Personas = _PersonaRepository.GetAll();
            _Atenciones = atencionRepository.GetAll();
            _Citas = _CitaRepository.GetAll();

            Personas = new ObservableCollection<LookupItem>(
                _Personas
                .OrderBy(p => p.Nombre)
                .Select(_PersonaToLookupItemFunc));
            
            Atenciones = new ObservableCollection<LookupItem>(
                _Atenciones
                 .OrderBy(a => a.Fecha)
                 .Select(_AtencionToLookupItemFunc));

            //foreach(var proximaCita in _Citas)
            //{
            //    proximaCita.Persona.Imagen = Personas.Where(p => p.Id == proximaCita.Persona.Id).First().Imagen;
            //}

            ProximasCitas = new ObservableCollection<LookupItem>(
                _Citas
                 .OrderBy(c => c.Fecha)
                 //.Where(c => c.Fecha >= DateTime.Now.Date)
                 .Select(_CitaToLookupItemFunc));

            SelectPersonaCommand = new DelegateCommand<LookupItem>(OnSelectPersonaCommandExecute);
            SelectCitaCommand = new DelegateCommand<LookupItem>(OnSelectCitaCommandExecute);
            SelectAtencionCommand = new DelegateCommand<LookupItem>(OnSelectAtencionCommandExecute);

            FiltrarPorPersonaCommand = new DelegateCommand<object>(OnFiltrarPorPersonaCommandExecute);

            _EventAggregator.GetEvent<PersonaCreadaEvent>().Subscribe(OnPersonaCreadaEvent);
            _EventAggregator.GetEvent<AtencionCreadaEvent>().Subscribe(OnAtencionCreadaEvent);
            _EventAggregator.GetEvent<CitaCreadaEvent>().Subscribe(OnNuevaCitaEvent);
            _EventAggregator.GetEvent<PersonaEliminadaEvent>().Subscribe(OnPersonaEliminadaEvent);

            _EventAggregator.GetEvent<PersonaActualizadaEvent>().Subscribe(OnPersonaActualizadaEvent);
        }

        private LookupItem _PersonaSeleccionada;
        public LookupItem PersonaSeleccionada
        {
            get { return _PersonaSeleccionada; }
            set
            {
                _PersonaSeleccionada = value;
                OnPropertyChanged(nameof(PersonaSeleccionada));
            }
        }


        public ObservableCollection<LookupItem> Atenciones { get; private set; }
        public ObservableCollection<LookupItem> ProximasCitas { get; private set; }
        public ObservableCollection<LookupItem> Personas { get; private set; }

        public ICommand SelectPersonaCommand { get; private set; }
        public ICommand SelectCitaCommand { get; private set; }
        public ICommand SelectAtencionCommand { get; private set; }
        public ICommand FiltrarPorPersonaCommand { get; private set; }

        Func<Persona, LookupItem> _PersonaToLookupItemFunc = persona => new LookupItem
        {
            Id = persona.Id,
            DisplayMember1 = persona.Nombre,
            DisplayMember2 = persona.Nif,
            Imagen = persona.Imagen
        };

        Func<Atencion, LookupItem> _AtencionToLookupItemFunc = atencion => new LookupItem
        {
            Id = atencion.Id,
            DisplayMember1 = atencion.Fecha.ToString(),
            DisplayMember2 = LookupItem.ShortenStringForDisplay(atencion.Seguimiento, 30),
            IconSource = @"atencion_icon.png",
            Imagen = BinaryImageConverter.GetBitmapImageFromUriSource(
                             new Uri("pack://application:,,,/Gama.Atenciones.Wpf;component/Resources/Images/atencion_icon.png")),
        };

        Func<Cita, LookupItem> _CitaToLookupItemFunc = cita => new LookupItem
        {
            Id = cita.Id,
            DisplayMember1 = cita.Fecha.ToString(),
            DisplayMember2 = cita.Sala,
            IconSource = cita.Persona.AvatarPath,
            Imagen = cita.Persona.Imagen
        };

        private void OnFiltrarPorPersonaCommandExecute(object parameter)
        {
            if (!_FiltradoEstaActivo)
            {
                var personaSeleccionada = parameter as LookupItem;

                Personas = new ObservableCollection<LookupItem>(
                              _Personas
                              .Where(p => p.Id == personaSeleccionada.Id)
                               .OrderBy(p => p.Nombre)
                              .Select(_PersonaToLookupItemFunc));

                Atenciones = new ObservableCollection<LookupItem>(
                    _Atenciones
                    .Where(a => a.Cita.Persona.Id == personaSeleccionada.Id)
                    .OrderBy(a => a.Fecha)
                    .Select(_AtencionToLookupItemFunc));

                ProximasCitas = new ObservableCollection<LookupItem>(
                    _Citas
                    .Where(c => c.Persona.Id == personaSeleccionada.Id)
                    .OrderBy(c => c.Fecha)
                    .Select(_CitaToLookupItemFunc));

                // En este caso siempre quedará solo una persona, pues se está filtrando
                // individualmente, por eso seleccionamos el único que hay, para que se
                // resalte en la interfaz.
                PersonaSeleccionada = Personas[0];

                _FiltradoEstaActivo = true;
            }
            else
            {
                Personas = new ObservableCollection<LookupItem>(
                           _Personas
                           .OrderBy(p => p.Nombre)
                           .Select(_PersonaToLookupItemFunc));

                Atenciones = new ObservableCollection<LookupItem>(
                    _Atenciones
                    .OrderBy(a => a.Fecha)
                    .Select(_AtencionToLookupItemFunc));

                ProximasCitas = new ObservableCollection<LookupItem>(
                    _Citas
                    .OrderBy(c => c.Fecha)
                    .Select(_CitaToLookupItemFunc));

                _FiltradoEstaActivo = false;
            }

            OnPropertyChanged(nameof(Personas));
            OnPropertyChanged(nameof(Atenciones));
            OnPropertyChanged(nameof(ProximasCitas));
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

            _Personas.Add(persona);
            Personas.Insert(0, lookupItem);
        }

        private void OnPersonaEliminadaEvent(int id)
        {
            //
            // Últimas personas
            //
            Personas.Remove(Personas.First(x => x.Id == id));

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
            
            for (int i = 0; i < Atenciones.Count; i++)
            {
                if (atencionesIds.Contains(Atenciones[i].Id))
                {
                    Atenciones.RemoveAt(i);
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
            Atenciones.Insert(0, lookupItem);
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
                // Ha habido que hacerlo así porque por alguna razón no se estaba
                // recogiendo bien la imagen desde la persona de dentro de la Cita
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

            var personaDesactualizada = Personas.Where(x => x.Id == id).FirstOrDefault();
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

        #region Gráficas que no se usan 
        //private int _MesInicialPersonas;
        //private string[] _Labels;
        //private int _MesInicialAtenciones;
        //private string[] _LabelsTotales;
        //public ChartValues<int> PersonasNuevasPorMes { get; private set; }
        //public ChartValues<int> AtencionesNuevasPorMes { get; private set; }
        //public ChartValues<int> Totales { get; private set; }

        //public string[] PersonasLabels =>
        //    _Labels.Skip(_MesInicialPersonas)
        //        .Take(_Settings.DashboardMesesAMostrarDePersonasNuevas).ToArray();

        //public string[] AtencionesLabels =>
        //    _Labels.Skip(_MesInicialAtenciones)
        //        .Take(_Settings.DashboardMesesAMostrarDeAtencionesNuevas).ToArray();

        //public string[] TotalesLabels => _LabelsTotales;

        //private void InicializarGraficos()
        //{
        //    _Labels = new[] {
        //        "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago","Sep","Oct", "Nov", "Dic",
        //        "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic", };
        //    _LabelsTotales = new[] { "Personas", "Citas", "Atenciones" };

        //    _MesInicialPersonas = 12 + (DateTime.Now.Month - 1) - _Settings.DashboardMesesAMostrarDePersonasNuevas + 1;
        //    _MesInicialAtenciones = 12 + (DateTime.Now.Month - 1) - _Settings.DashboardMesesAMostrarDeAtencionesNuevas + 1;

        //    PersonasNuevasPorMes = new ChartValues<int>(_PersonaRepository.GetPersonasNuevasPorMes(
        //               _Settings.DashboardMesesAMostrarDePersonasNuevas));

        //    AtencionesNuevasPorMes = new ChartValues<int>(_AtencionRepository.GetAtencionesNuevasPorMes(
        //               _Settings.DashboardMesesAMostrarDeAtencionesNuevas));

        //    Totales = new ChartValues<int>(
        //        new int[] {
        //            _PersonaRepository.CountAll(),
        //            _CitaRepository.CountAll(),
        //            _AtencionRepository.CountAll()
        //        });
        //}
        #endregion
    }
}
