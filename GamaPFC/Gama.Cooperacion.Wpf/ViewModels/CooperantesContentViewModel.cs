using Core;
using Gama.Cooperacion.Business;
using Gama.Cooperacion.Wpf.Eventos;
using Gama.Cooperacion.Wpf.Services;
using Gama.Cooperacion.Wpf.Views;
using Gama.Cooperacion.Wpf.Wrappers;
using NHibernate;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gama.Cooperacion.Wpf.ViewModels
{
    public class CooperantesContentViewModel : ViewModelBase
    {
        private ICooperanteRepository _CooperanteRepository;
        private IActividadRepository _ActividadRepository;
        private EventAggregator _EventAggregator;
        private ISession _Session;
        private DateTime _Now;
        private bool _VisibleListaCooperantes;
        private CooperanteViewModel _CooperanteViewModel;
        private CooperanteWrapper _CooperanteSeleccionado;

        public CooperantesContentViewModel(
            EventAggregator eventAggregator,
            IActividadRepository actividadRepository,
            ICooperanteRepository cooperanteRepository,
            CooperanteViewModel CooperanteViewModel,
            ISession session)
        {
            Gama.Common.Debug.Debug.StartWatch();
            _EventAggregator = eventAggregator;
            _CooperanteRepository = cooperanteRepository;
            _CooperanteRepository.Session = session;
            _ActividadRepository = actividadRepository;
            _ActividadRepository.Session = session;
            _CooperanteViewModel = CooperanteViewModel;
            _Session = session;

            _Now = DateTime.Now.Date;
            _VisibleListaCooperantes = true;

            OnActualizarServidor();

            HabilitarBotonEditarCommand = new DelegateCommand(
                OnHabilitarBotonEditarCommand,
                () => !_CooperanteViewModel.Cooperante.IsInEditionMode);

            GuardarBotonCommand = new DelegateCommand(
                OnGuardarBotonCommand,
                () =>
                _CooperanteViewModel.Cooperante.IsInEditionMode
                   && _CooperanteViewModel.Cooperante.IsChanged
                   && _CooperanteViewModel.Cooperante.IsValid
                   );

            CancelarBotonEditarCommand = new DelegateCommand(OnCancelarEdicionCommand,
                () => _CooperanteViewModel.Cooperante.IsInEditionMode);

            _EventAggregator.GetEvent<CooperanteCreadoEvent>().Subscribe(OnCooperanteCreadoEvent);
            _EventAggregator.GetEvent<ActividadActualizadaEvent>().Subscribe(OnActividadActualizadaEvent);
            _EventAggregator.GetEvent<ActividadEliminadaEvent>().Subscribe(OnActividadEliminadaEvent);
            _EventAggregator.GetEvent<ActividadCreadaEvent>().Subscribe(OnActividadNuevaEvent);

            BorrarActividadCommand = new DelegateCommand<object>(OnBorrarActividadCommandExecute);
            EditarActividadCommand = new DelegateCommand<object>(OnEditarActividadCommandExecute);
            SelectActividadCommand = new DelegateCommand<object>(OnSelectActividadCommand);
            ExportarCooperanteView = new DelegateCommand(OnExportarCooperanteViewCommand);
            VerListaCooperantesCommand = new DelegateCommand(OnVerListaCooperantesCommand);
            
            Gama.Common.Debug.Debug.StopWatch("CooperantesContentViewModel");

        }
        //-------------------------------------------------
        // Observable Collections
        //-------------------------------------------------
        public ObservableCollection<CooperanteWrapper> Cooperantes { get; private set; }
        public ObservableCollection<Actividad> ListaCompletaActividades { get; private set; }
        public ObservableCollection<Actividad> ActividadesCoordina { get; private set; }
        public ObservableCollection<Actividad> ActividadesCoopera { get; private set; }

        //--------------------------
        // Events
        //--------------------------
        private void OnActividadActualizadaEvent(int id)
        {
            var actividadActualizada = _ActividadRepository.GetById(id);
            if (ActividadesCoordina.Any(a => a.Id == id))
            {
                var indice = ActividadesCoordina.IndexOf(ActividadesCoordina.Single(a => a.Id == id));
                ActividadesCoordina.RemoveAt(indice);
                ActividadesCoordina.Insert(0, actividadActualizada);

            }
            if (ActividadesCoopera.Any(a => a.Id == id))
            {
                var indice = ActividadesCoopera.IndexOf(ActividadesCoopera.Single(a => a.Id == id));
                ActividadesCoopera.RemoveAt(indice);
                ActividadesCoopera.Insert(0, actividadActualizada);

            }
        }
        private void OnActividadEliminadaEvent(Actividad obj)
        {
            ActividadesCoopera.Remove(obj);
            ActividadesCoordina.Remove(obj);

        }
        private void OnActividadNuevaEvent(int id)
        {
            var actividad = _ActividadRepository.GetById(id);
            ListaCompletaActividades.Add(actividad);
            RefrescarVista();
        }
        private void OnCooperanteCreadoEvent(CooperanteWrapper param)
        {
            Cooperantes.Insert(0, param);
        }

        //--------------------------------
        // Commands Interfaces
        //--------------------------------
        public ICommand BorrarActividadCommand { get; set; }
        public ICommand CancelarBotonEditarCommand { get; private set; }
        public ICommand EditarActividadCommand { get; set; }
        public ICommand GuardarBotonCommand { get; private set; }
        public ICommand HabilitarBotonEditarCommand { get; private set; }
        public ICommand SelectActividadCommand { get; set; }
        public ICommand ExportarCooperanteView { get; set; }
        public ICommand VerListaCooperantesCommand { get; set; }


        private void OnExportarCooperanteViewCommand()
        {
            _EventAggregator.GetEvent<CooperanteSeleccionadoEvent>().Publish(CooperanteSeleccionado.Id);
        }


        private void OnBorrarActividadCommandExecute(object param)
        {
            var lookup = param as Actividad;
            _ActividadRepository.Delete(lookup);
            _EventAggregator.GetEvent<ActividadEliminadaEvent>().Publish(lookup);

        }
        private void OnCancelarEdicionCommand()
        {
            _CooperanteViewModel.Cooperante.RejectChanges();
            _CooperanteViewModel.Cooperante.IsInEditionMode = false;
        }
        private void OnEditarActividadCommandExecute(object param)
        {
            var lookup = param as Actividad;
            var o = new NuevaActividadView();
            var vm = (NuevaActividadViewModel)o.DataContext;
            vm.Load(lookup);
            o.Title = "Editar Actividad";
            o.ShowDialog();
        }
        private void OnGuardarBotonCommand()
        {
            _CooperanteViewModel.Cooperante.UpdatedAt = DateTime.Now;
            _CooperanteRepository.Update(_CooperanteViewModel.Cooperante.Model);
            _CooperanteViewModel.Cooperante.AcceptChanges();
            _CooperanteViewModel.Cooperante.IsInEditionMode = false;
        }
        private void OnHabilitarBotonEditarCommand()
        {
            _CooperanteViewModel.Cooperante.IsInEditionMode = true;
        }
        private void OnSelectActividadCommand(object param)
        {
            var lookup = param as Actividad;
            if (lookup != null) _EventAggregator.GetEvent<ActividadSeleccionadaEvent>().Publish(lookup.Id);
        }
        private void OnVerListaCooperantesCommand()
        {
            if (VisibleListaCooperantes == true) VisibleListaCooperantes = false;
            else VisibleListaCooperantes = true;
        }

        //----------------------
        // Bindings
        //----------------------
        public CooperanteWrapper CooperanteSeleccionado
        {
            get { return _CooperanteSeleccionado; }
            set
            {
                SetProperty(ref _CooperanteSeleccionado, value);
                RefrescarVista();
                _EventAggregator.GetEvent<CooperanteSeleccionadoEvent>().Publish(_CooperanteSeleccionado.Id);
                OnPropertyChanged();
            }
        }
        public CooperanteViewModel CooperanteViewModel
        {
            get { return _CooperanteViewModel; }
            set { SetProperty(ref _CooperanteViewModel, value); }
        }

        //---------------------------
        // Visibilities Properties
        //---------------------------
        public bool VisibleListaCooperantes
        {
            get { return _VisibleListaCooperantes; }
            set { SetProperty(ref _VisibleListaCooperantes, value); }
        }

        //---------------------------
        // Funciones/procedimientos 
        //---------------------------
        public override void OnActualizarServidor()
        {
            Cooperantes = new ObservableCollection<CooperanteWrapper>(
                _CooperanteRepository.GetAll()
                .Select(x => new CooperanteWrapper(x)));
            ListaCompletaActividades = new ObservableCollection<Actividad>(
                _ActividadRepository.GetAll());

            ActividadesCoordina = new ObservableCollection<Actividad>();
            ActividadesCoopera = new ObservableCollection<Actividad>();

            if (Cooperantes.Count > 0)
            {
                CooperanteSeleccionado = Cooperantes.First();
            }
              
            OnPropertyChanged(nameof(Cooperantes));
        }
        private void RefrescarVista()
        {
            if (_CooperanteSeleccionado != null)
            {
                _CooperanteViewModel.Load(_CooperanteSeleccionado);
                _CooperanteSeleccionado.PropertyChanged += (s, e) => InvalidateCommands();

                ActividadesCoordina.Clear();
                foreach (var Act in ListaCompletaActividades)
                {
                    if (Act.Coordinador.Id == _CooperanteSeleccionado.Id)
                    {
                        ActividadesCoordina.Add(Act);
                    }
                }
                ActividadesCoopera.Clear();
                foreach (var Act in ListaCompletaActividades)
                {
                    foreach (var ActCoo in Act.Cooperantes)
                    {
                        if (ActCoo.Id == _CooperanteSeleccionado.Id)
                        {
                            ActividadesCoopera.Add(Act);
                        }
                    }
                }

            }
        }
      
        //-----------------------------
        // InvalidateCommands
        //-----------------------------
        private void InvalidateCommands()
        {
            ((DelegateCommand)HabilitarBotonEditarCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)GuardarBotonCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)CancelarBotonEditarCommand).RaiseCanExecuteChanged();
        }
    }
}
