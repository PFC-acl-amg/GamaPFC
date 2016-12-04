using Core;
using Gama.Common;
using Gama.Socios.Wpf.Eventos;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Socios.Wpf.ViewModels
{
    public class SociosContentViewModel : ViewModelBase
    {
        private IEventAggregator _EventAggregator;
        private IRegionManager _RegionManager;

        public SociosContentViewModel(IEventAggregator eventAggregator,
            IRegionManager regionManager)
        {
            _EventAggregator = eventAggregator;
            _RegionManager = regionManager;

            _EventAggregator.GetEvent<SocioSeleccionadoEvent>().Subscribe(AbrirSocio);
            _EventAggregator.GetEvent<SocioCreadoEvent>().Subscribe(AbrirSocio);
        }

        private void AbrirSocio(int id)
        {
            var navigationParameters = new NavigationParameters();
            navigationParameters.Add("Id", id);
            _RegionManager.RequestNavigate(RegionNames.ContentRegion, "SociosContentView", navigationParameters);
            _RegionManager.RequestNavigate(RegionNames.SociosTabContentRegion, "EditarSocioView", navigationParameters);
        }
    }
}
