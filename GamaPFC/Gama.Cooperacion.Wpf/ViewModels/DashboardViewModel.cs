using Core;
using Gama.Cooperacion.Business;
using Gama.Cooperacion.Wpf.Eventos;
using Gama.Cooperacion.Wpf.Services;
using NHibernate;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {
        private IActividadRepository _actividadRepository;
        private int _cantidadDeProyectosAMostrar = 5;
        private IEventAggregator _eventAggregator;
        private ICooperanteRepository _cooperanteRepository;
        private int _cantidadDeCooperantesAMostrar = 10;

        public ObservableCollection<Actividad> UltimasActividades { get; private set; }
        public ObservableCollection<Cooperante> UltimosCooperantes { get; private set; }

        public DashboardViewModel(IActividadRepository actividadRepository,
            ICooperanteRepository cooperanteRepository,
            IEventAggregator eventAggregator, ISession session)
        {
            _actividadRepository = actividadRepository;
            _cooperanteRepository = cooperanteRepository;
            _actividadRepository.Session = session;
            _cooperanteRepository.Session = session;
            _eventAggregator = eventAggregator;


            UltimasActividades = new ObservableCollection<Actividad>(
                _actividadRepository.GetAll()
                .OrderBy(a => a.FechaDeFin)
                .Take(_cantidadDeProyectosAMostrar)
                .ToArray());

            UltimosCooperantes = new ObservableCollection<Cooperante>(
                _cooperanteRepository.GetAll()
                .OrderBy(c => c.Id)
                .Take(_cantidadDeCooperantesAMostrar)
                .ToArray());

            _eventAggregator.GetEvent<NuevaActividadEvent>().Subscribe(OnNuevaActividadEvent);
        }

        private void OnNuevaActividadEvent(int id)
        {
            UltimasActividades.Add(_actividadRepository.GetById(id));
        }
    }
}
