using Core;
using Gama.Atenciones.Wpf.Services;
using Gama.Common.CustomControls;
using Gama.Atenciones.Wpf.Eventos;
using NHibernate;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Gama.Atenciones.Business;
using Gama.Atenciones.Wpf.UIEvents;

namespace Gama.Atenciones.Wpf.ViewModels
{

    public class SearchBoxViewModel : ViewModelBase
    {
        private IPersonaRepository _PersonaRepository;
        private IEventAggregator _EventAggregator;
        private string _TextoDeBusqueda;
        private LookupItem _UltimaPersonaSeleccionada;
        private IEnumerable _MensajeDeEspera;
        private IEnumerable _ResultadoDeBusqueda;

        public SearchBoxViewModel(
            IPersonaRepository personaRepository,
            IEventAggregator eventAggregator,
            ISession session)
        {
            AtencionesResources.StartStopWatch();
            _PersonaRepository = personaRepository;
            _PersonaRepository.Session = session;
            _EventAggregator = eventAggregator;
            _MensajeDeEspera = new List<string>() { "Espera por favor..." };

            SearchCommand = new DelegateCommand(OnSearchCommandExecute);
            SelectResultCommand = new DelegateCommand(OnSelectResultCommandExecute);

            Personas = new ObservableCollection<LookupItem>(_PersonaRepository.GetAllForLookup());
            AtencionesResources.StopStopWatch("SearchBoxView: GetAllForLookup()");
            AtencionesResources.StartStopWatch();

            _EventAggregator.GetEvent<PersonaCreadaEvent>().Subscribe(OnPersonaCreadaEvent);
            _EventAggregator.GetEvent<PersonaActualizadaEvent>().Subscribe(OnPersonaActualizadaEvent);
            AtencionesResources.StopStopWatch("SearchBoxView");
        }

        public string TextoDeBusqueda
        {
            get { return _TextoDeBusqueda; }
            set { SetProperty(ref _TextoDeBusqueda, value); }
        }

        public LookupItem UltimaPersonaSeleccionada
        {
            get { return _UltimaPersonaSeleccionada; }
            set { SetProperty(ref _UltimaPersonaSeleccionada, value); }
        }

        public IEnumerable MensajeDeEspera => _MensajeDeEspera;
        public IEnumerable ResultadoDeBusqueda => _ResultadoDeBusqueda;
        public ObservableCollection<LookupItem> Personas { get; private set; }

        public ICommand SearchCommand { get; private set; }
        public ICommand SelectResultCommand { get; private set; }

        public override void OnActualizarServidor()
        {
            Personas = new ObservableCollection<LookupItem>(_PersonaRepository.GetAllForLookup());
        }

        private void OnSearchCommandExecute()
        {
            _EventAggregator.GetEvent<PersonaEnBusquedaEvent>().Publish(TextoDeBusqueda);

            _ResultadoDeBusqueda =
                Personas.Where(p => p.DisplayMember1.ToLower().Contains(TextoDeBusqueda.Trim().ToLower()));

            OnPropertyChanged(nameof(ResultadoDeBusqueda));
        }

        private void OnSelectResultCommandExecute()
        {
            _EventAggregator.GetEvent<PersonaSeleccionadaEvent>().Publish(_UltimaPersonaSeleccionada.Id);
        }

        private void OnPersonaCreadaEvent(int id)
        {
            var persona = _PersonaRepository.GetById(id);

            Personas.Add(new LookupItem
            {
                DisplayMember1 = persona.Nombre,
                DisplayMember2 = persona.Nif,
                Imagen = persona.Imagen
            });
        }

        private void OnPersonaActualizadaEvent(int id)
        {
            var persona = _PersonaRepository.GetById(id);
            _PersonaRepository.Session.Evict(persona);

            var personaDesactualizada = Personas.Where(x => x.Id == id).FirstOrDefault();

            if (personaDesactualizada != null)
            {
                personaDesactualizada.DisplayMember1 = persona.Nombre;
                personaDesactualizada.DisplayMember2 = persona.Nif;
                personaDesactualizada.Imagen = persona.Imagen;
            }
        }
    }
}
