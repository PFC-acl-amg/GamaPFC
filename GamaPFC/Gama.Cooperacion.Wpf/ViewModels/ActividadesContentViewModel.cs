using Core;
using Gama.Common;
using Gama.Common.Eventos;
using Gama.Cooperacion.Wpf.Eventos;
using Gama.Cooperacion.Wpf.Services;
using Gama.Cooperacion.Wpf.Views;
using MahApps.Metro.Controls;
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
using System.Windows.Controls;
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

            Views = new ObservableCollection<MetroTabItem>();
            AddView(container.Resolve<ListadoDeActividadesView>(), listadoDeActividadesViewModel);

            CloseTabCommand = new DelegateCommand<MetroTabItem>(OnCloseTabCommandExecute);

            _EventAggregator.GetEvent<ActividadCreadaEvent>().Subscribe(OnActividadNuevaEvent);
            _EventAggregator.GetEvent<ActividadSeleccionadaEvent>().Subscribe(OnActividadSeleccionadaEvent);
        }

        private MetroTabItem AddView(EditarActividadView view, EditarActividadViewModel viewModel)
        {
            var tabItem = new MetroTabItem();
            tabItem.DataContext = viewModel;
            tabItem.Content = view;
            Views.Add(tabItem);
            return tabItem;
        }

        private MetroTabItem AddView(ListadoDeActividadesView view, ListadoDeActividadesViewModel viewModel)
        {
            var tabItem = new MetroTabItem();
            tabItem.DataContext = viewModel;
            tabItem.Content = view;
            Views.Add(tabItem);
            return tabItem;
        }

        private ObservableCollection<MetroTabItem> _Views;
        public ObservableCollection<MetroTabItem> Views
        {
            get { return _Views; }
            set
            {
                _Views = value;
                value.CollectionChanged += delegate
                {
                    OnPropertyChanged("Views");
                };

                OnPropertyChanged("Views");
            }
        }

        public ICommand CloseTabCommand { get; private set; }

        private void OnCloseTabCommandExecute(MetroTabItem tabItemACerrar)
        {
            var viewModel = tabItemACerrar.DataContext as IConfirmarPeticionDeNavegacion;

            if (viewModel.ConfirmarPeticionDeNavegacion())
                Views.Remove(tabItemACerrar);
        }

        private int _SelectedIndex;
        public int SelectedIndex
        {
            get { return _SelectedIndex; }
            set { SetProperty(ref _SelectedIndex, value); }
        }

        private object _ViewSeleccionada;
        public object ViewSeleccionada
        {
            get { return _ViewSeleccionada; }
            set
            {
                SetProperty(ref _ViewSeleccionada, value);
                SetActiveTab();
            }
        }

        private void SetActiveTab()
        {
            foreach (var view in Views)
            {
                if (view.Content as ListadoDeActividadesView != null)
                    continue;

                if (view == ViewSeleccionada)
                    ((IActiveAware)view.DataContext).IsActive = true;
                else
                    ((IActiveAware)view.DataContext).IsActive = false;
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
                var newView = _Container.Resolve<EditarActividadView>();
                newView.DataContext = newViewModel;

                newViewModel.OnNavigatedTo(id);
                
                ViewSeleccionada = AddView(newView, newViewModel);
            }

            _EventAggregator.GetEvent<ActiveViewChanged>().Publish("ActividadesContentView");
        }

        private bool ActividadEstaAbierta(int actividadId)
        {
            try
            {
                foreach (var view in Views)
                {
                    var editarActividadView = view.Content as EditarActividadView;
                    if (editarActividadView != null)
                    {
                        if (((EditarActividadViewModel)editarActividadView.DataContext).InformacionDeActividadViewModel.Actividad.Id == actividadId)
                        {
                            ((EditarActividadViewModel)editarActividadView.DataContext).OnNavigatedTo(actividadId);
                            ViewSeleccionada = view;
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
