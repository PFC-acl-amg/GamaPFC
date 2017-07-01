using Core;
using Core.Util;
using Gama.Atenciones.Business;
using Gama.Atenciones.Wpf.Converters;
using Gama.Atenciones.Wpf.Eventos;
using Gama.Atenciones.Wpf.Services;
using Gama.Atenciones.Wpf.UIEvents;
using Gama.Common.CustomControls;
using Gama.Common.Eventos;
using NHibernate;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Gama.Atenciones.Wpf.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {
        private IAtencionRepository _AtencionRepository;
        private ICitaRepository _CitaRepository;
        private IEventAggregator _EventAggregator;
        private IPersonaRepository _PersonaRepository;
        public Preferencias Preferencias { get; private set; }
        private List<Persona> _Personas;
        private List<Cita> _Citas;
        private List<Atencion> _Atenciones;
        private bool _FiltradoEstaActivo = false;
        private string _TextoDeBusqueda = "";

        public DashboardViewModel(IPersonaRepository personaRepository,
            ICitaRepository citaRepository,
            IAtencionRepository atencionRepository,
            IEventAggregator eventAggregator,
            Preferencias settings,
            ISession session)
        {
            AtencionesResources.StartStopWatch();
            _PersonaRepository = personaRepository;
            _CitaRepository = citaRepository;
            _AtencionRepository = atencionRepository;
            _EventAggregator = eventAggregator;
            Preferencias = settings;

            _PersonaRepository.Session = session;
            _CitaRepository.Session = session;
            _AtencionRepository.Session = session;

            _Personas = new List<Persona>(_PersonaRepository.GetAll());
            //_Citas = new List<Cita>(_Personas.SelectMany(p => p.Citas));
            //_Atenciones = new List<Atencion>(_Citas.Select(c => c.Atencion));
            _Atenciones = new List<Atencion>(_AtencionRepository.GetAll());
            _Citas = new List<Cita>(_CitaRepository.GetAll());

            AtencionesResources.StopStopWatch("...Dashboard: Get Personas, Atenciones, Citas");
            AtencionesResources.StartStopWatch();
            Personas = new ObservableCollection<Persona>(
                _Personas
                .OrderBy(p => p.Nombre)
                .ToList());
            
            Atenciones = new ObservableCollection<Atencion>(
                _Atenciones
                .OrderBy(a => a.Fecha)
                .Select(_AtencionToFullAtencion));

            ProximasCitas = new ObservableCollection<Cita>(
                _Citas
                .OrderBy(c => c.Fecha)
                .ToList());

            AtencionesResources.StopStopWatch("...Dashboard: Crear colecciones para la vista");
            SelectPersonaCommand = new DelegateCommand<Persona>(OnSelectPersonaCommandExecute);
            SelectCitaCommand = new DelegateCommand<Cita>(OnSelectCitaCommandExecute);
            SelectAtencionCommand = new DelegateCommand<Atencion>(OnSelectAtencionCommandExecute);
            FiltrarPorPersonaCommand = new DelegateCommand<object>(OnFiltrarPorPersonaCommandExecute);
            ResetearFechasCommand = new DelegateCommand(() =>
            {
                _FechaDeInicio = null;
                _FechaDeFin = null;
                FiltrarPorFecha();
            });

            _EventAggregator.GetEvent<PersonaCreadaEvent>().Subscribe(OnPersonaCreadaEvent);
            _EventAggregator.GetEvent<PersonaActualizadaEvent>().Subscribe(OnPersonaActualizadaEvent);
            _EventAggregator.GetEvent<PersonaEliminadaEvent>().Subscribe(OnPersonaEliminadaEvent);
            _EventAggregator.GetEvent<PersonaEnBusquedaEvent>().Subscribe(OnPersonaEnBusquedaEvent);

            _EventAggregator.GetEvent<CitaCreadaEvent>().Subscribe(OnCitaCreadaEvent);
            _EventAggregator.GetEvent<CitaActualizadaEvent>().Subscribe(OnCitaActualizadaEvent);
            _EventAggregator.GetEvent<CitaEliminadaEvent>().Subscribe(OnCitaEliminadaEvent);

            _EventAggregator.GetEvent<AtencionCreadaEvent>().Subscribe(OnAtencionCreadaEvent);
            _EventAggregator.GetEvent<AtencionActualizadaEvent>().Subscribe(OnAtencionActualizadaEvent);
            _EventAggregator.GetEvent<AtencionEliminadaEvent>().Subscribe(OnAtencionEliminadaEvent);


            DoMagicCommand = new DelegateCommand(OnDoMagicCommandExecute);
            StopMagicCommand = new DelegateCommand(OnStopMagicCommandExecute);
            AtencionesResources.StopStopWatch("DashboardView");
        }

        private Persona _PersonaSeleccionada;
        public Persona PersonaSeleccionada
        {
            get { return _PersonaSeleccionada; }
            set
            {
                _PersonaSeleccionada = value;
                OnPropertyChanged(nameof(PersonaSeleccionada));
            }
        }

        private DateTime? _FechaDeInicio;
        public DateTime? FechaDeInicio
        {
            get { return _FechaDeInicio; }
            set { SetProperty(ref _FechaDeInicio, value); FiltrarPorFecha(); }
        }

        private DateTime? _FechaDeFin;
        public DateTime? FechaDeFin
        {
            get { return _FechaDeFin; }
            set { SetProperty(ref _FechaDeFin, value); FiltrarPorFecha(); }
        }

        // Estas listas son dinámicas en tanto que según el filtro de fecha que se aplique
        // (a través de FechaDeInicio y FechaDeFin) contendrán un conjunto diferentes de 
        // elementos. Las listas que contienen todos los elementos son las privadas 
        // (_Personas, _Citas y _Atenciones)
        public ObservableCollection<Persona> Personas { get; private set; }
        public ObservableCollection<Atencion> Atenciones { get; private set; }
        public ObservableCollection<Cita> ProximasCitas { get; private set; }

        public ICommand SelectPersonaCommand { get; private set; }
        public ICommand SelectCitaCommand { get; private set; }
        public ICommand SelectAtencionCommand { get; private set; }
        public ICommand FiltrarPorPersonaCommand { get; private set; }
        public ICommand ResetearFechasCommand { get; private set; }
        
        private Func<Atencion, Atencion> _AtencionToFullAtencion =
            atencion =>
            { 
                atencion.Seguimiento = LookupItem.ShortenStringForDisplay(atencion.Seguimiento, 30);
                //atencion.Imagen = Converters.BinaryImageConverter.GetBitmapImageFromUriSource(
                //                 new Uri("pack://application:,,,/Gama.Atenciones.Wpf;component/Resources/Images/add atencion.png"));
                return atencion;
            };

        /// 
        /// FUNCIONES PRIVADAS Y MÉTODOS PÚBLICOS
        /// 
        

        // Filtra por fecha si éstas están definidas en la interfaz.
        // Si no, carga todos los elementos. Cada vez que hay algún cambio
        // en personas, citas o atenciones, se llamará a esta función para
        // refrescar la vista.
        private void FiltrarPorFecha()
        {
            var fechaDeInicio = FechaDeInicio ?? DateTime.Now.AddYears(-100);
            var fechaDeFin = FechaDeFin ?? DateTime.Now.AddYears(10);

            //Personas.Clear();
            //Personas.AddRange(_Personas
            //    .Where(p => p.CreatedAt.IsBetween(FechaDeInicio, FechaDeFin)
            //        || p.UpdatedAt.IsBetween(FechaDeInicio, FechaDeFin)
            //        || p.Citas.Any(c => c.Fecha.IsBetween(FechaDeInicio, FechaDeFin)))
            //    .Where(p => p.Nombre.ToLower().Contains(_TextoDeBusqueda.Trim().ToLower()))
            //    .OrderBy(p => p.Nombre)
            //    .ToList());
            Personas = new ObservableCollection<Persona>(
                _Personas
                .Where(p => p.CreatedAt.IsBetween(FechaDeInicio, FechaDeFin)
                    || p.UpdatedAt.IsBetween(FechaDeInicio, FechaDeFin)
                    || p.Citas.Any(c => c.Fecha.IsBetween(FechaDeInicio, FechaDeFin)))
                .Where(p => p.Nombre.ToLower().Contains(_TextoDeBusqueda.Trim().ToLower()))
                .OrderBy(p => p.Nombre)
                .ToList());

            //Atenciones.Clear();
            //Atenciones.AddRange(_Atenciones
            //    .Where(x => x.Fecha.IsBetween(FechaDeInicio, FechaDeFin))
            //    .OrderBy(a => a.Fecha)
            //    .Select(_AtencionToFullAtencion));
            Atenciones = new ObservableCollection<Atencion>(
                _Atenciones
                .Where(x => x.Fecha.IsBetween(FechaDeInicio, FechaDeFin))
                .OrderBy(a => a.Fecha)
                .Select(_AtencionToFullAtencion));

            //ProximasCitas.Clear();
            //ProximasCitas.AddRange(_Citas
            //    .Where(x => x.Fecha.IsBetween(FechaDeInicio, FechaDeFin))
            //    .OrderBy(c => c.Fecha)
            //    .ToList());
            ProximasCitas = new ObservableCollection<Cita>(
                _Citas
                .Where(x => x.Fecha.IsBetween(FechaDeInicio, FechaDeFin))
                .OrderBy(c => c.Fecha)
                .ToList());

            OnPropertyChanged(nameof(Personas));
            OnPropertyChanged(nameof(Atenciones));
            OnPropertyChanged(nameof(ProximasCitas));
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            _EventAggregator.GetEvent<ActiveViewChanged>().Publish("DashboardView");
        }

        /// 
        /// FUNCIONES DE LOS COMANDOS
        /// 

        private void OnFiltrarPorPersonaCommandExecute(object parameter)
        {
            if (!_FiltradoEstaActivo)
            {
                var personaSeleccionada = parameter as Persona;
                
                // Ocurre cuando se hace click en la lista pero no sobre una persona
                if (personaSeleccionada == null) return; 

                Personas = new ObservableCollection<Persona>(
                    _Personas
                    .Where(p => p.Id == personaSeleccionada.Id)
                    .ToList());

                Atenciones = new ObservableCollection<Atencion>(
                    _Atenciones
                    .Where(a => a.Cita.Persona.Id == personaSeleccionada.Id)
                    .Where(x => x.Fecha.IsBetween(FechaDeInicio, FechaDeFin))
                    .OrderBy(a => a.Fecha)
                    .Select(_AtencionToFullAtencion));

                ProximasCitas = new ObservableCollection<Cita>(
                    _Citas
                    .Where(c => c.Persona.Id == personaSeleccionada.Id)
                    .Where(x => x.Fecha.IsBetween(FechaDeInicio, FechaDeFin))
                    .OrderBy(c => c.Fecha)
                    .ToList());

                // En este caso siempre quedará solo una persona, pues se está filtrando
                // individualmente, por eso seleccionamos el único que hay, para que se
                // resalte en la interfaz.
                PersonaSeleccionada = Personas[0];

                _FiltradoEstaActivo = true;
            }
            // Si el filtro estaba activado, lo que se hace al hacer click otra vez
            // es volver a mostrar a todos, teniendo en cuenta el filtro de búsqueda
            else
            {
                FiltrarPorFecha();
                _FiltradoEstaActivo = false;
            }

            OnPropertyChanged(nameof(Personas));
            OnPropertyChanged(nameof(Atenciones));
            OnPropertyChanged(nameof(ProximasCitas));
        }

        private void OnSelectPersonaCommandExecute(Persona persona)
        {
            _EventAggregator.GetEvent<PersonaSeleccionadaEvent>().Publish(persona.Id);
        }

        private void OnSelectAtencionCommandExecute(Atencion atencion)
        {
            _EventAggregator.GetEvent<AtencionSeleccionadaEvent>().Publish(
                new IdentificadorDeModelosPayload
                {
                    PersonaId = atencion.Cita.Persona.Id,
                    CitaId = atencion.Cita.Id,
                    AtencionId = atencion.Id
                });
        }

        private void OnSelectCitaCommandExecute(Cita cita)
        {
            _EventAggregator.GetEvent<CitaSeleccionadaEvent>().Publish(cita.Persona.Id);
        }

        ///
        /// FUNCIONES DE LOS EVENTOS
        /// 

        private void OnPersonaEnBusquedaEvent(string textoDeBusqueda)
        {
            _TextoDeBusqueda = textoDeBusqueda;

            Personas = new ObservableCollection<Persona>(
                _Personas
                .Where(p => p.Nombre.ToLower().Contains(textoDeBusqueda.Trim().ToLower()))
                .OrderBy(p => p.Nombre)
                .ToList());

            // Si al filtrar sólo queda una persona, se activará el filtro 
            // de como si se hubiera seleccionada a esa persona
            if (Personas.Count == 1)
            {
                PersonaSeleccionada = Personas.First();
                OnFiltrarPorPersonaCommandExecute(PersonaSeleccionada);
            }

            OnPropertyChanged(nameof(Personas));
        }

        private void OnPersonaCreadaEvent(int id)
        {
            var persona = _PersonaRepository.GetById(id);
            _Personas.Add(persona);
            FiltrarPorFecha();
        }

        private void OnPersonaActualizadaEvent(int personaId)
        {
            var persona = _PersonaRepository.GetById(personaId);
            _Personas.Remove(_Personas.Find(x => x.Id == personaId));
            _Personas.Add(persona);

            var citasDesactualizadas = _Citas.Where(x => x.Persona.Id == personaId).ToList();
            foreach (var citaDesactualizada in citasDesactualizadas)
                citaDesactualizada.Persona.Imagen = persona.Imagen;

            FiltrarPorFecha();
        }

        private void OnPersonaEliminadaEvent(int id)
        {
            _Personas.Remove(_Personas.First(x => x.Id == id));
            _Citas.RemoveAll(x => x.Persona.Id == id);
            _Atenciones.RemoveAll(x => x.Cita.Persona.Id == id);
            FiltrarPorFecha();
        }

        private void OnAtencionCreadaEvent(int id)
        {
            var atencion = _AtencionRepository.GetById(id);
            _Atenciones.Add(atencion);
            FiltrarPorFecha();
        }

        private void OnAtencionActualizadaEvent(int atencionId)
        {
            var atencion = _AtencionRepository.GetById(atencionId);
            _Atenciones.Remove(_Atenciones.Find(x => x.Id == atencionId));
            _Atenciones.Add(atencion);
            FiltrarPorFecha();
        }

        private void OnAtencionEliminadaEvent(int atencionId)
        {
            _Atenciones.Remove(_Atenciones.Find(x => x.Id == atencionId));
            FiltrarPorFecha();
        }

        private void OnCitaCreadaEvent(int id)
        {
            var cita = _CitaRepository.GetById(id);
            _Citas.Add(cita);
            FiltrarPorFecha();
        }

        private void OnCitaActualizadaEvent(int citaId)
        {
            var cita = _CitaRepository.GetById(citaId);
            _Citas.Remove(_Citas.Find(x => x.Id == citaId));
            _Citas.Add(cita);
            FiltrarPorFecha();
        }

        private void OnCitaEliminadaEvent(int citaId)
        {
            _Citas.Remove(_Citas.Find(x => x.Id == citaId));
            _Atenciones.RemoveAll(x => x.Cita.Id == citaId);
            FiltrarPorFecha();
        }

        int _Contador = 0;
        public ICommand DoMagicCommand { get; private set; }
        public ICommand StopMagicCommand { get; private set; }

        private void OnDoMagicCommandExecute()
        {
            AtencionesResources.ClientService.EnviarMensaje("uohh ihh jiji");
        }

        private void OnStopMagicCommandExecute()
        {
            AtencionesResources.ClientService.Desconectar();
        }

        public override void OnActualizarServidor()
        {
            _Personas = new List<Persona>(_PersonaRepository.GetAll());
            _Atenciones = new List<Atencion>(_AtencionRepository.GetAll());
            _Citas = new List<Cita>(_CitaRepository.GetAll());

            FiltrarPorFecha();
        }
    }
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