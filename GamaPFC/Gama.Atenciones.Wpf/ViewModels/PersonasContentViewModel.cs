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
            var navigationParameters = new NavigationParameters();
            navigationParameters.Add("Id", payload.PersonaId);

            if (payload.AtencionId.HasValue)
            {
                navigationParameters.Add("AtencionId", payload.AtencionId.Value);
            }

            _RegionManager.RequestNavigate(RegionNames.ContentRegion, "PersonasContentView", navigationParameters);
            _RegionManager.RequestNavigate(RegionNames.PersonasTabContentRegion, "EditarPersonaView", navigationParameters);
        }

        private void OnPersonaSeleccionadaEvent(int id)
        {
            AbrirPersona(id);
        }

        private void OnPersonaCreadaEvent(int id)
        {
            //AbrirPersona(id);
            //ISSUE Workaround porque al crear una persona por alguna razón, la navegación no funciona
            // el RequestNavigate
            var navigationParameters = new NavigationParameters();
            navigationParameters.Add("Id", id);
            var region = _RegionManager.Regions[RegionNames.PersonasTabContentRegion];
            var view = _Container.Resolve<EditarPersonaView>();
            var vm = (EditarPersonaViewModel)view.DataContext;
            region.Add(view);

            _RegionManager.RequestNavigate(RegionNames.ContentRegion, "PersonasContentView", navigationParameters);
            region.Activate(view);

            vm.Load(id);
        }

        private void AbrirPersona(int id)
        {
            OnPersonaCreadaEvent(id);
            //return;
            //var navigationParameters = new NavigationParameters();
            //navigationParameters.Add("Id", id);

            //// Primero cambiamos de panel, si no estamos ya en él
            //if (!IsActive)
            //{
            //    _RegionManager.RequestNavigate(RegionNames.ContentRegion, "PersonasContentView");
            //}

            //try
            //{
            //    // _RegionManager.RequestNavigate(RegionNames.ContentRegion, "PersonasContentView");
            //    //Segundamente navegamos al detalle de la persona a abrir
            //    //_RegionManager.RequestNavigate(RegionNames.ContentRegion, "PersonasContentView", navigationParameters);
            //    _RegionManager.RequestNavigate(RegionNames.PersonasTabContentRegion, "EditarPersonaView", navigationParameters);
            //}
            //catch (Exception eX)
            //{
            //    throw eX;
            //}
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
