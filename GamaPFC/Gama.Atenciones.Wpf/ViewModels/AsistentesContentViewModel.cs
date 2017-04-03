using Gama.Atenciones.Wpf.Eventos;
using Gama.Atenciones.Wpf.Services;
using Gama.Atenciones.Wpf.Wrappers;
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
    public class AsistentesContentViewModel
    {
        private IAsistenteRepository _AsistenteRepository;
        private IEventAggregator _EventAggregator;

        public AsistentesContentViewModel(
            IEventAggregator eventAggregator,
            IAsistenteRepository asistenteRepository,
            ISession session)
        {
            _EventAggregator = eventAggregator;
            _AsistenteRepository = asistenteRepository;
            _AsistenteRepository.Session = session;

            Asistentes = new ObservableCollection<AsistenteWrapper>(
                _AsistenteRepository.GetAll()
                .Select(x => new AsistenteWrapper(x)));

            _EventAggregator.GetEvent<AsistenteCreadoEvent>().Subscribe(OnAsistenteCreadoEvent);
        }

        public ObservableCollection<AsistenteWrapper> Asistentes { get; private set; }

        private void OnAsistenteCreadoEvent(int id)
        {
            var asistente = _AsistenteRepository.GetById(id);
            var wrapper = new AsistenteWrapper(asistente);

            Asistentes.Insert(0, wrapper);
        }
    }
}
