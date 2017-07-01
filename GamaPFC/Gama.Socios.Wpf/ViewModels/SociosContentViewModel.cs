using Core;
using Gama.Common;
using Gama.Common.Eventos;
using Gama.Socios.Wpf.Eventos;
using Microsoft.Practices.Unity;
using Prism;
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

namespace Gama.Socios.Wpf.ViewModels
{
    public class SociosContentViewModel : ViewModelBase
    {
        private EventAggregator _EventAggregator;
        private IUnityContainer _Container;

        public SociosContentViewModel(
            EventAggregator eventAggregator,
            ListadoDeSociosViewModel listadoDeSociosViewModel,
            IUnityContainer container)
        {
            _EventAggregator = eventAggregator;
            _Container = container;

            ViewModels = new ObservableCollection<object>();
            ViewModels.Add(listadoDeSociosViewModel);
            ViewModelSeleccionado = ViewModels.First();

            CloseTabCommand = new DelegateCommand<EditarSocioViewModel>(OnCloseTabCommandExecute);

            _EventAggregator.GetEvent<SocioSeleccionadoEvent>().Subscribe((id) => NavegarASocio(id));
            _EventAggregator.GetEvent<SocioCreadoEvent>().Subscribe((id) => NavegarASocio(id));
        }

        private void OnCloseTabCommandExecute(EditarSocioViewModel viewModelACerrar)
        {
            if (viewModelACerrar.ConfirmNavigationRequest())
                ViewModels.Remove(viewModelACerrar);
        }

        public ICommand CloseTabCommand { get; private set; }

        private object _ViewModelSeleccionado;
        public object ViewModelSeleccionado
        {
            get { return _ViewModelSeleccionado; }
            set
            {
                SetProperty(ref _ViewModelSeleccionado, value);
                SetActiveTab();
            }
        }

        public ObservableCollection<object> ViewModels { get; private set; }

        public override void OnActualizarServidor()
        {
            foreach (var viewModel in ViewModels)
                ((ViewModelBase)viewModel).OnActualizarServidor();
        }

        private void SetActiveTab()
        {
            foreach (var viewModel in ViewModels)
            {
                var activeAwareViewModel = viewModel as IActiveAware;
                if (activeAwareViewModel != null)
                {
                    if (activeAwareViewModel == ViewModelSeleccionado)
                        activeAwareViewModel.IsActive = true;
                    else
                        activeAwareViewModel.IsActive = false;
                }
            }
        }

        private void NavegarASocio(int socioId)
        {
            if (!SocioEstaAbierto(socioId))
            {
                var newViewModel = _Container.Resolve<EditarSocioViewModel>();

                ViewModels.Add(newViewModel);

                newViewModel.OnNavigatedTo(socioId);

                ViewModelSeleccionado = newViewModel;
            }

            _EventAggregator.GetEvent<ActiveViewChanged>().Publish("SociosContentView");
        }

        private bool SocioEstaAbierto(int socioId)
        {
            try
            {
                foreach (var viewModel in ViewModels)
                {
                    var editarSocioViewModel = viewModel as EditarSocioViewModel;
                    if (editarSocioViewModel != null)
                    {
                        if (editarSocioViewModel.Socio.Id == socioId)
                        {
                            editarSocioViewModel.OnNavigatedTo(socioId);
                            ViewModelSeleccionado = editarSocioViewModel;
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;

            }
            return false;
        }
    }
}
