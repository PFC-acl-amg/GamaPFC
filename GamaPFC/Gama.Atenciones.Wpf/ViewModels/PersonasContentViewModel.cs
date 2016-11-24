using Core;
using Gama.Atenciones.Wpf.Eventos;
using Gama.Common;
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

        public PersonasContentViewModel(IEventAggregator eventAggregator,
            IRegionManager regionManager)
        {
            _EventAggregator = eventAggregator;
            _RegionManager = regionManager;

            _EventAggregator.GetEvent<PersonaCreadaEvent>().Subscribe(OnNuevaPersonaEvent);
            _EventAggregator.GetEvent<PersonaSeleccionadaEvent>().Subscribe(OnPersonaSeleccionadaEvent);
        }

        private void OnPersonaSeleccionadaEvent(int id)
        {
            AbrirPersona(id);
        }

        private void OnNuevaPersonaEvent(int id)
        {
            AbrirPersona(id);
        }

        private void AbrirPersona(int id)
        {
            var navigationParameters = new NavigationParameters();
            navigationParameters.Add("Id", id);

            // Primero cambiamos de panel
            _RegionManager.RequestNavigate(RegionNames.ContentRegion, "PersonasContentView");

            try {
                // Segundamente navegamos al detalle de la Actividad a abrir
                _RegionManager.RequestNavigate(RegionNames.PersonasTabContentRegion,
                    "EditarPersonaView", navigationParameters);
            }
            catch (Exception eX)
            {
                throw eX;
            }
        }
    }
}
