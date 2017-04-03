using Gama.Atenciones.Wpf.Services;
using Gama.Atenciones.Wpf.Wrappers;
using NHibernate;
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

        public AsistentesContentViewModel(
            IAsistenteRepository asistenteRepository,
            ISession session)
        {
            _AsistenteRepository = asistenteRepository;
            _AsistenteRepository.Session = session;

            Asistentes = new ObservableCollection<AsistenteWrapper>(
                _AsistenteRepository.GetAll()
                .Select(x => new AsistenteWrapper(x)));
        }

        public ObservableCollection<AsistenteWrapper> Asistentes { get; private set; }
    }
}
