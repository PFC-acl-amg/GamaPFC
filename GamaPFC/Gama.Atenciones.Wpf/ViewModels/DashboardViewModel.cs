using Core;
using Gama.Atenciones.Wpf.Services;
using Gama.Common.CustomControls;
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
        private IEventAggregator _EventAggregator;
        private IPersonaRepository _PersonaRepository;
        private IAtencionesSettings _Settings;

        public DashboardViewModel(IPersonaRepository personaRepository,
            IEventAggregator eventAggregator,
            IAtencionesSettings settings,
            ISession session)
        {
            _PersonaRepository = personaRepository;
            _PersonaRepository.Session = session;
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
        }

        public ObservableCollection<LookupItem> UltimasPersonas { get; private set; }
    }
}
