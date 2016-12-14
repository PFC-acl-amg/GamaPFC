using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using Gama.Socios.Wpf.Wrappers;
using Prism.Events;
using Gama.Socios.Wpf.Services;

namespace Gama.Socios.Wpf.ViewModels
{
    public class EditarPeriodosDeAltaViewModel : ViewModelBase
    {
        private IEventAggregator _EventAggregator;
        private ISocioRepository _SocioRepository;
        private ISession _Session;

        public EditarPeriodosDeAltaViewModel(
            ISocioRepository socioRepository,
            IEventAggregator eventAggregator)
        {
            _SocioRepository = socioRepository;
            _EventAggregator = eventAggregator;
        }

        public void Load(SocioWrapper socio)
        {
            Socio = socio;
        }

        public SocioWrapper Socio { get; set; }

        public ISession Session
        {
            get { return _Session; }
            set
            {
                _Session = value;
                _SocioRepository.Session = value;
            }
        }
    }
}
