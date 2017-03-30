using Core;
using Gama.Common.CustomControls;
using Gama.Socios.Wpf.Eventos;
using Gama.Socios.Wpf.Services;
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

namespace Gama.Socios.Wpf.ViewModels
{

    public class SearchBoxViewModel : ViewModelBase
    {
        private ISocioRepository _SocioRepository;
        private IEventAggregator _EventAggregator;
        private string _TextoDeBusqueda;
        private LookupItem _UltimoSocioSeleccionado;
        private IEnumerable _MensajeDeEspera;
        private IEnumerable _ResultadoDeBusqueda;

        public SearchBoxViewModel(
            ISocioRepository socioRepository,
            IEventAggregator eventAggregator,
            ISession session)
        {
            _SocioRepository = socioRepository;
            _SocioRepository.Session = session;
            _EventAggregator = eventAggregator;
            _MensajeDeEspera = new List<string>() { "Espera por favor..." };

            SearchCommand = new DelegateCommand(OnSearchCommandExecute);
            SelectResultCommand = new DelegateCommand(OnSelectResultCommandExecute);

            Socios = new ObservableCollection<LookupItem>(socioRepository.GetAllForLookup());

            _EventAggregator.GetEvent<SocioCreadoEvent>().Subscribe(OnSocioCreadoEvent);
        }

        public string TextoDeBusqueda
        {
            get { return _TextoDeBusqueda; }
            set { SetProperty(ref _TextoDeBusqueda, value); }
        }

        public LookupItem UltimoSocioSeleccionado
        {
            get { return _UltimoSocioSeleccionado; }
            set { SetProperty(ref _UltimoSocioSeleccionado, value); }
        }

        public IEnumerable MensajeDeEspera => _MensajeDeEspera;
        public IEnumerable ResultadoDeBusqueda => _ResultadoDeBusqueda;
        public ObservableCollection<LookupItem> Socios { get; private set; }

        public ICommand SearchCommand { get; private set; }
        public ICommand SelectResultCommand { get; private set; }

        private void OnSearchCommandExecute()
        {
            _ResultadoDeBusqueda = Socios.Where(
                p => p.DisplayMember1.ToLower().Contains(TextoDeBusqueda.Trim().ToLower()));
            OnPropertyChanged(nameof(ResultadoDeBusqueda));
        }

        private void OnSelectResultCommandExecute()
        {
            _EventAggregator.GetEvent<SocioSeleccionadoEvent>().Publish(_UltimoSocioSeleccionado.Id);
        }

        private void OnSocioCreadoEvent(int id)
        {
            var socio = _SocioRepository.GetById(id);
            Socios.Add(new LookupItem { DisplayMember1 = socio.Nombre, DisplayMember2 = socio.Nif, Id = socio.Id });
        }
    }
}
