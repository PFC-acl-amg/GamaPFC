using Core;
using Gama.Atenciones.Business;
using Gama.Atenciones.Wpf.Services;
using Gama.Atenciones.Wpf.UIEvents;
using Gama.Atenciones.Wpf.Wrappers;
using NHibernate;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Atenciones.Wpf.ViewModels
{
    public class CitasContentViewModel : ViewModelBase
    {
        private ICitaRepository _CitaRepository;
        private IEventAggregator _EventAggregator;

        public CitasContentViewModel(
            IEventAggregator eventAggregator,
            ICitaRepository citaRepository,
            ISession session)
        {
            _EventAggregator = eventAggregator;
            _CitaRepository = citaRepository;
            _CitaRepository.Session = session;

            Citas = new ObservableCollection<CitaWrapper>(
                _CitaRepository.GetAll()
                .Select(x => new CitaWrapper(x)));
        }

        public ObservableCollection<CitaWrapper> Citas { get; private set; }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            _EventAggregator.GetEvent<ActiveViewChanged>().Publish("CitasContentView");
        }
    }
}
