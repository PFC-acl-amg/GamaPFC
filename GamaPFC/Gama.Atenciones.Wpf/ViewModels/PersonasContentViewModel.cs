using Core;
using Gama.Atenciones.Wpf.Eventos;
using Gama.Atenciones.Wpf.UIEvents;
using Gama.Atenciones.Wpf.Views;
using Gama.Common;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Atenciones.Wpf.ViewModels
{
    public class PersonasContentViewModel : ViewModelBase
    {
        private IEventAggregator _EventAggregator;
        private IRegionManager _RegionManager;
        private IUnityContainer _Container;

        public PersonasContentViewModel(IEventAggregator eventAggregator,
            IRegionManager regionManager, IUnityContainer container)
        {
            _EventAggregator = eventAggregator;
            _RegionManager = regionManager;
            _Container = container;

            _EventAggregator.GetEvent<PersonaCreadaEvent>().Subscribe(OnPersonaCreadaEvent);
            _EventAggregator.GetEvent<PersonaSeleccionadaEvent>().Subscribe(OnPersonaSeleccionadaEvent);
            _EventAggregator.GetEvent<CitaSeleccionadaEvent>().Subscribe(OnPersonaSeleccionadaEvent);
            _EventAggregator.GetEvent<AtencionSeleccionadaEvent>().Subscribe(OnAtencionSeleccionadaEvent);
        }

        private void OnAtencionSeleccionadaEvent(IdentificadorDeModelosPayload payload)
        {
            NavegarAPersona(payload.PersonaId, payload.AtencionId);
        }

        private void OnPersonaSeleccionadaEvent(int id)
        {
            NavegarAPersona(id);
        }

        private void OnPersonaCreadaEvent(int id)
        {
            NavegarAPersona(id);
        }

        private void NavegarAPersona(int personaId, int? atencionId = null)
        {
            //ISSUE Workaround porque al crear una persona por alguna razón, la navegación no funciona
            // el RequestNavigate

            var region = _RegionManager.Regions[RegionNames.PersonasTabContentRegion];
            var navigationContext = new NavigationContext(region.NavigationService, null);
            navigationContext.Parameters.Add("Id", personaId);
            if (atencionId.HasValue)
            {
                navigationContext.Parameters.Add("AtencionId", atencionId.Value);
            }

            var views = region.Views;
            foreach (var existingView in views)
            {
                var editarPersonaView = existingView as EditarPersonaView;
                if (editarPersonaView != null)
                {
                    var editarPersonaViewModel = (EditarPersonaViewModel)editarPersonaView.DataContext;
                    if (editarPersonaViewModel.IsNavigationTarget(navigationContext))
                    {
                        editarPersonaViewModel.OnNavigatedTo(navigationContext);
                        region.Activate(existingView);
                        _RegionManager.RequestNavigate(RegionNames.ContentRegion, "PersonasContentView");
                        return;
                    }
                }
            }
            
            var newView = _Container.Resolve<EditarPersonaView>();
            var vm = (EditarPersonaViewModel)newView.DataContext;
            vm.OnNavigatedTo(navigationContext);
            region.Add(newView);

            _RegionManager.RequestNavigate(RegionNames.ContentRegion, "PersonasContentView");
            region.Activate(newView);
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            IsActive = false;
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            IsActive = true;
            _EventAggregator.GetEvent<ActiveViewChanged>().Publish("PersonasContentView");
        }
    }
}

