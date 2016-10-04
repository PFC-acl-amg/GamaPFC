using Core;
using Gama.Common;
using Gama.Cooperacion.Wpf.Eventos;
using NHibernate;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.ViewModels
{
    public class ActividadesContentViewModel : ViewModelBase
    {
        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        private EditarActividadViewModel _selectedViewModel;
        //private bool _cerrar = false;

        public ActividadesContentViewModel(IEventAggregator eventAggregator,
            IRegionManager regionManager)
        {
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;

            _eventAggregator.GetEvent<NuevaActividadEvent>().Subscribe(OnActividadNuevaEvent);
            _eventAggregator.GetEvent<ActividadSeleccionadaEvent>().Subscribe(OnActividadSeleccionadaEvent);
        }

        public EditarActividadViewModel SelectedViewModel
        {
            get { return _selectedViewModel; }
            set
            {
                _selectedViewModel = value;
                if (_selectedViewModel != null)
                {
                    _eventAggregator.GetEvent<ActividadSeleccionadaChangedEvent>().
                        Publish(_selectedViewModel.Actividad);
                }
            }
        }

        private void OnActividadNuevaEvent(int id)
        {
            AbrirActividad(id);
        }

        private void OnActividadSeleccionadaEvent(int id)
        {
            AbrirActividad(id);
        }

        private void AbrirActividad(int id)
        {
            var navigationParameters = new NavigationParameters();
            navigationParameters.Add("Id", id);

            // Primero cambiamos de panel
            _regionManager.RequestNavigate(RegionNames.ContentRegion, "ActividadesContentView");

            // Segundamente navegamos al detalle de la Actividad a abrir
            _regionManager.RequestNavigate(RegionNames.ActividadesTabContentRegion,
                "EditarActividadView", navigationParameters);
        }

        //private void OnActividadEliminadaEvent(ActividadWrapper obj)
        //{
        //    Cerrar = true;
        //}
    }
}
