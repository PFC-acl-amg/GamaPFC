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
        private IAtencionRepository _AtencionRepository;
        private ICitaRepository _CitaRepository;
        private IEventAggregator _EventAggregator;
        private IPersonaRepository _PersonaRepository;
        private IAtencionesSettings _Settings;

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
            _PersonaRepository.Session = session;
            _CitaRepository.Session = session;
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

            UltimasCitas = new ObservableCollection<LookupItem>(
                _CitaRepository.GetAll()
                 .OrderBy(c => c.Inicio)
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
        }

        public ObservableCollection<LookupItem> UltimasAtenciones { get; private set; }
        public ObservableCollection<LookupItem> UltimasCitas { get; private set; }
        public ObservableCollection<LookupItem> UltimasPersonas { get; private set; }
    }
}
