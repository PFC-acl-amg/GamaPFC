using Core;
using Gama.Atenciones.Wpf.Eventos;
using Gama.Atenciones.Wpf.UIEvents;
using Gama.Atenciones.Wpf.Views;
using Gama.Common;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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

            EditarPersonaViewModels = new ObservableCollection<object>();
            EditarPersonaViewModels.Add(_Container.Resolve<ListadoDePersonasViewModel>());
            ViewModelSeleccionado = EditarPersonaViewModels.First();
            SelectedIndex = 0;

            CloseTabCommand = new DelegateCommand<EditarPersonaViewModel>(OnCloseTabCommandExecute);

            _EventAggregator.GetEvent<PersonaCreadaEvent>().Subscribe(OnPersonaCreadaEvent);
            _EventAggregator.GetEvent<PersonaSeleccionadaEvent>().Subscribe(OnPersonaSeleccionadaEvent);
            _EventAggregator.GetEvent<CitaSeleccionadaEvent>().Subscribe(OnPersonaSeleccionadaEvent);
            _EventAggregator.GetEvent<AtencionSeleccionadaEvent>().Subscribe(OnAtencionSeleccionadaEvent);
            _EventAggregator.GetEvent<PersonaEliminadaEvent>().Subscribe(OnPersonaEliminadaEvent);
        }

        private void OnCloseTabCommandExecute(EditarPersonaViewModel viewModelACerrar)
        {
            if (viewModelACerrar.ConfirmNavigationRequest())
                EditarPersonaViewModels.Remove(viewModelACerrar);
        }

        public ICommand CloseTabCommand { get; private set; }

        private object _ViewModelSeleccionado;
        public object ViewModelSeleccionado
        {
            get { return _ViewModelSeleccionado; }
            set { SetProperty(ref _ViewModelSeleccionado, value); }
        }

        private int _SelectedIndex;
        public int SelectedIndex
        {
            get { return _SelectedIndex; }
            set { SetProperty(ref _SelectedIndex, value); }
        }

        public ObservableCollection<object> EditarPersonaViewModels { get; private set; }

        private void NavegarAPersona2(int personaId, int? atencionId = null)
        {
            if (!PersonaEstaAbierta(personaId, atencionId))
            {
                var newViewModel = _Container.Resolve<EditarPersonaViewModel>();

                EditarPersonaViewModels.Add(newViewModel);

                newViewModel.OnNavigatedTo(personaId, atencionId);

                ViewModelSeleccionado = newViewModel;
            }
            
            _EventAggregator.GetEvent<ActiveViewChanged>().Publish("PersonasContentView");
        }

        private bool PersonaEstaAbierta(int personaId, int? atencionId)
        {
            foreach (var viewModel in EditarPersonaViewModels)
            {
                var editarPersonaViewModel = viewModel as EditarPersonaViewModel;
                if (editarPersonaViewModel != null)
                {
                    if (editarPersonaViewModel.Persona.Id == personaId)
                    {
                        editarPersonaViewModel.OnNavigatedTo(personaId, atencionId);
                        ViewModelSeleccionado = editarPersonaViewModel;
                        return true;
                    }
                }
            }

            return false;
        }

        private void NavegarAPersona(int personaId, int? atencionId = null)
        {
            NavegarAPersona2(personaId, atencionId);
            //return;
            ////ISSUE Workaround porque al crear una persona por alguna razón, la navegación no funciona
            //// el RequestNavigate

            //var region = _RegionManager.Regions[RegionNames.PersonasTabContentRegion];
            //var navigationContext = new NavigationContext(region.NavigationService, null);
            //navigationContext.Parameters.Add("Id", personaId);
            //if (atencionId.HasValue)
            //{
            //    navigationContext.Parameters.Add("AtencionId", atencionId.Value);
            //}

            //var views = region.Views;
            //foreach (var existingView in views)
            //{
            //    var editarPersonaView = existingView as EditarPersonaView;
            //    if (editarPersonaView != null)
            //    {
            //        var editarPersonaViewModel = (EditarPersonaViewModel)editarPersonaView.DataContext;
            //        if (editarPersonaViewModel.IsNavigationTarget(navigationContext))
            //        {
            //            editarPersonaViewModel.OnNavigatedTo(navigationContext);
            //            region.Activate(existingView);
            //            _RegionManager.RequestNavigate(RegionNames.ContentRegion, "PersonasContentView");
            //            return;
            //        }
            //    }
            //}
            
            //var newView = _Container.Resolve<EditarPersonaView>();
            //var vm = (EditarPersonaViewModel)newView.DataContext;
            //vm.OnNavigatedTo(navigationContext);
            //region.Add(newView);

            //_EventAggregator.GetEvent<ActiveViewChanged>().Publish("PersonasContentView");
            ////_RegionManager.RequestNavigate(RegionNames.ContentRegion, "PersonasContentView");
            //region.Activate(newView);
        }

        private void OnPersonaEliminadaEvent(int id)
        {
            var region = _RegionManager.Regions[RegionNames.PersonasTabContentRegion];
            var navigationContext = new NavigationContext(region.NavigationService, null);
            var views = region.Views;
            foreach (var existingView in views)
            {
                var editarPersonaView = existingView as EditarPersonaView;
                if (editarPersonaView != null)
                {
                    var editarPersonaViewModel = (EditarPersonaViewModel)editarPersonaView.DataContext;
                    if (editarPersonaViewModel.Persona.Id == id)
                    {
                        region.Remove(editarPersonaView);
                        break;
                    }
                }
            }

            foreach (var existingView in views)
            {
                var listadoDePersonasView = existingView as ListadoDePersonasView;
                if (listadoDePersonasView != null)
                {
                    region.Activate(listadoDePersonasView);
                }
            }
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

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            _EventAggregator.GetEvent<ActiveViewChanged>().Publish("PersonasContentView");
        }
    }
}

