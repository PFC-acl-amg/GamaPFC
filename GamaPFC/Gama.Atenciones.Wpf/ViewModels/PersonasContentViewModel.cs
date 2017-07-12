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
using Gama.Atenciones.Wpf.Wrappers;
using Prism;
using Gama.Atenciones.Wpf.Services;
using NHibernate;
using Gama.Common.Eventos;
using Gama.Common.Debug;
using MahApps.Metro.Controls;

namespace Gama.Atenciones.Wpf.ViewModels
{
    public class PersonasContentViewModel : ViewModelBase
    {
        private EventAggregator _EventAggregator;
        private IUnityContainer _Container;

        public PersonasContentViewModel(
            ICitaRepository citaRepository,
            IPersonaRepository personaRepository,
            EventAggregator eventAggregator,     
            ListadoDePersonasViewModel listadoDePersonasViewModel,
            IUnityContainer container,
            ISession session)
        {
            Debug.StartWatch();
            _CitaRepository = citaRepository;
            _PersonaRepository = personaRepository;
            _PersonaRepository.Session = session;
            _CitaRepository.Session = session;
            _EventAggregator = eventAggregator;
            _Container = container;

            Views = new ObservableCollection<MetroTabItem>();
            AddView(container.Resolve<ListadoDePersonasView>(), listadoDePersonasViewModel);

            CloseTabCommand = new DelegateCommand<MetroTabItem>(OnCloseTabCommandExecute);

            _EventAggregator.GetEvent<PersonaCreadaEvent>().Subscribe(OnPersonaCreadaEvent);
            _EventAggregator.GetEvent<PersonaSeleccionadaEvent>().Subscribe(OnPersonaSeleccionadaEvent);
            _EventAggregator.GetEvent<CitaSeleccionadaEvent>().Subscribe(OnCitaSeleccionadaEvent);
            _EventAggregator.GetEvent<AtencionSeleccionadaEvent>().Subscribe(OnAtencionSeleccionadaEvent);
            _EventAggregator.GetEvent<CitaCreadaEvent>().Subscribe(OnCitaCreadaEvent);
            _EventAggregator.GetEvent<CitaActualizadaEvent>().Subscribe(OnCitaActualizadaEvent);
            _EventAggregator.GetEvent<PersonaEliminadaEvent>().Subscribe(OnPersonaEliminadaEvent);
            _EventAggregator.GetEvent<NuevaAtencionEvent>().Subscribe(OnNuevaAtencionEvent);
            Debug.StopWatch("PersonasContentView");
        }

        private MetroTabItem AddView(EditarPersonaView view, EditarPersonaViewModel viewModel)
        {
            var tabItem = new MetroTabItem();
            tabItem.DataContext = viewModel;
            tabItem.Content = view;
            Views.Add(tabItem);
            return tabItem;
        }

        private MetroTabItem AddView(ListadoDePersonasView view, ListadoDePersonasViewModel viewModel)
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

        private void OnCloseTabCommandExecute(MetroTabItem tabItemACerrar)
        {
            var viewModel = tabItemACerrar.DataContext as IConfirmarPeticionDeNavegacion;

            if (viewModel.ConfirmarPeticionDeNavegacion())
                Views.Remove(tabItemACerrar);
        }

        public ICommand CloseTabCommand { get; private set; }

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
                if (view == ViewSeleccionada)
                    ((IActiveAware)view.DataContext).IsActive = true;
                else
                    ((IActiveAware)view.DataContext).IsActive = false;
            }
        }
        public override void OnActualizarServidor()
        {
            foreach (var view in Views)
                ((ViewModelBase)(view.DataContext)).OnActualizarServidor();
        }

        private int _SelectedIndex;
        private ICitaRepository _CitaRepository;
        private IPersonaRepository _PersonaRepository;

        public int SelectedIndex
        {
            get { return _SelectedIndex; }
            set { SetProperty(ref _SelectedIndex, value); }
        }

        //public ObservableCollection<object> ViewModels { get; private set; }

        private void NavegarAPersona(int personaId, int? atencionId = null, DateTime? fechaDeCita = null)
        {
            if (!PersonaEstaAbierta(personaId, atencionId, fechaDeCita))
            {
                var newViewModel = _Container.Resolve<EditarPersonaViewModel>();
                var newView = _Container.Resolve<EditarPersonaView>();
                newView.DataContext = newViewModel;
                //ViewModels.Add(newViewModel);

                newViewModel.OnNavigatedTo(personaId, atencionId, fechaDeCita);

                ViewSeleccionada = AddView(newView, newViewModel);
            }

            _EventAggregator.GetEvent<ActiveViewChanged>().Publish("PersonasContentView");
        }

        private bool PersonaEstaAbierta(int personaId, int? atencionId = null, DateTime? fechaDeCita = null)
        {
            try
            {
                foreach (var view in Views)
                {
                    var editarPersonaView = view.Content as EditarPersonaView;
                    if (editarPersonaView != null)
                    {
                        if (((EditarPersonaViewModel)editarPersonaView.DataContext).PersonaVM.Persona.Id == personaId)
                        {
                            ((EditarPersonaViewModel)editarPersonaView.DataContext).OnNavigatedTo(personaId, atencionId, fechaDeCita);
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

        private void OnNuevaAtencionEvent(CitaWrapper wrapper)
        {
            try
            {
                int personaId = wrapper.Persona.Id;
                int? atencionId = null;
                if (wrapper.Atencion != null)
                    atencionId = wrapper.Atencion.Id;

                NavegarAPersona(personaId, atencionId);

                //if (!PersonaEstaAbierta(personaId, atencionId))
                //{
                //    var newViewModel = _Container.Resolve<EditarPersonaViewModel>();
                //    ViewModels.Add(newViewModel);
                //    newViewModel.OnNavigatedTo(personaId, atencionId);
                //    ViewModelSeleccionado = newViewModel;
                //}

                //_EventAggregator.GetEvent<ActiveViewChanged>().Publish("PersonasContentView");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void OnCitaCreadaEvent(int citaId)
        {
            var cita = _CitaRepository.GetById(citaId);
            int personaId = cita.Persona.Id;

            foreach (var view in Views)
            {
                var editarPersonaViewModel = view.DataContext as EditarPersonaViewModel;
                if (editarPersonaViewModel != null)
                {
                    if (editarPersonaViewModel.PersonaVM.Persona.Id == personaId)
                    {
                        editarPersonaViewModel.PersonaVM.Persona.AcceptChanges();
                        editarPersonaViewModel.CitasVM.Refresh++;
                        break;
                    }
                }
            }
        }

        private void OnCitaActualizadaEvent(int citaId)
        {
            var cita = _CitaRepository.GetById(citaId);
            int personaId = cita.Persona.Id;

            foreach (var view in Views)
            {
                var editarPersonaViewModel = view.DataContext as EditarPersonaViewModel;
                if (editarPersonaViewModel != null)
                {
                    if (editarPersonaViewModel.PersonaVM.Persona.Id == personaId)
                    {
                        editarPersonaViewModel.CitasVM.ActualizarCita(cita);
                        break;
                    }
                }
            }
        }

        private void OnPersonaEliminadaEvent(int id)
        {
            var persona = _PersonaRepository.GetById(id);
            int personaId = persona.Id;

            MetroTabItem tabToRemove = null;
            foreach (var view in Views)
            {
                var editarPersonaViewModel = view.DataContext as EditarPersonaViewModel;
                if (editarPersonaViewModel != null)
                {
                    if (editarPersonaViewModel.PersonaVM.Persona.Id == personaId)
                    {
                        tabToRemove = view;
                        break;
                    }
                }
            }

            if (tabToRemove != null)
                Views.Remove(tabToRemove);
        }

        private void OnAtencionSeleccionadaEvent(IdentificadorDeModelosPayload payload)
        {
            NavegarAPersona(payload.PersonaId, payload.AtencionId);
        }

        private void OnCitaSeleccionadaEvent(int id)
        {
            var cita = _CitaRepository.GetById(id);
            NavegarAPersona(cita.Persona.Id, null, cita.Fecha);
        }

        private void OnPersonaSeleccionadaEvent(int id)
        {
            NavegarAPersona(id);
            _EventAggregator.GetEvent<PersonaSeleccionadaChangedEvent>().Publish(id);
        }

        private void OnPersonaCreadaEvent(int id)
        {
            NavegarAPersona(id);
        }
    }
}


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



//var region = _RegionManager.Regions[RegionNames.PersonasTabContentRegion];
//var navigationContext = new NavigationContext(region.NavigationService, null);
//var views = region.Views;
//foreach (var existingView in views)
//{
//    var editarPersonaView = existingView as EditarPersonaView;
//    if (editarPersonaView != null)
//    {
//        var editarPersonaViewModel = (EditarPersonaViewModel)editarPersonaView.DataContext;
//        if (editarPersonaViewModel.Persona.Id == id)
//        {
//            region.Remove(editarPersonaView);
//            break;
//        }
//    }
//}

//foreach (var existingView in views)
//{
//    var listadoDePersonasView = existingView as ListadoDePersonasView;
//    if (listadoDePersonasView != null)
//    {
//        region.Activate(listadoDePersonasView);
//    }
//}