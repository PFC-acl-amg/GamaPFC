using Core;
using Gama.Common;
using Gama.Common.Eventos;
using Gama.Cooperacion.Wpf.Eventos;
using Gama.Cooperacion.Wpf.Services;
using Microsoft.Practices.Unity;
using NHibernate;
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

namespace Gama.Cooperacion.Wpf.ViewModels
{
    public class ActividadesContentViewModel : ViewModelBase
    {
        private EventAggregator _EventAggregator;
        private IActividadRepository _ActividadRepository;
        private IUnityContainer _Container;
        private ISession _Session;

        public ActividadesContentViewModel(
            IActividadRepository actividadRepository,
            ListadoDeActividadesViewModel listadoDeActividadesViewModel,
            IUnityContainer container,
            ISession session,
            EventAggregator eventAggregator)
        {
            _ActividadRepository = actividadRepository;
            _EventAggregator = eventAggregator;
            _Container = container;
            _Session = session;
            
            _ActividadRepository.Session = session;

            ViewModels = new ObservableCollection<object>();
            ViewModels.Add(listadoDeActividadesViewModel);
            ViewModelSeleccionado = ViewModels.First();
            SelectedIndex = 0;

            CloseTabCommand = new DelegateCommand<EditarActividadViewModel>(OnCloseTabCommandExecute);

            _EventAggregator.GetEvent<ActividadCreadaEvent>().Subscribe(OnActividadNuevaEvent);
            _EventAggregator.GetEvent<ActividadSeleccionadaEvent>().Subscribe(OnActividadSeleccionadaEvent);
        }

        public ObservableCollection<object> ViewModels { get; private set; }

        public ICommand CloseTabCommand { get; private set; }

        private void OnCloseTabCommandExecute(EditarActividadViewModel viewModelACerrar)
        {
            if (viewModelACerrar.ConfirmNavigationRequest())
                ViewModels.Remove(viewModelACerrar);
        }

        private int _SelectedIndex;
        public int SelectedIndex
        {
            get { return _SelectedIndex; }
            set { SetProperty(ref _SelectedIndex, value); }
        }

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

        private void OnActividadNuevaEvent(int id)
        {
            NavegarAActividad(id);
        }

        private void OnActividadSeleccionadaEvent(int id)
        {
            NavegarAActividad(id);
            _EventAggregator.GetEvent<ActividadSeleccionadaChangedEvent>().Publish(id);
        }

        private void NavegarAActividad(int id)
        {
            if (!ActividadEstaAbierta(id))
            {
                var newViewModel = _Container.Resolve<EditarActividadViewModel>();

                ViewModels.Add(newViewModel);

                newViewModel.OnNavigatedTo(id);

                ViewModelSeleccionado = newViewModel;
            }

            _EventAggregator.GetEvent<ActiveViewChanged>().Publish("ActividadesContentView");
        }

        private bool ActividadEstaAbierta(int actividadId)
        {
            try
            {
                foreach (var viewModel in ViewModels)
                {
                    var editarActividadViewModel = viewModel as EditarActividadViewModel;
                    if (editarActividadViewModel != null)
                    {
                        if (editarActividadViewModel.InformacionDeActividadViewModel.Actividad.Id == actividadId)
                        {
                            editarActividadViewModel.OnNavigatedTo(actividadId);
                            ViewModelSeleccionado = editarActividadViewModel;
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
