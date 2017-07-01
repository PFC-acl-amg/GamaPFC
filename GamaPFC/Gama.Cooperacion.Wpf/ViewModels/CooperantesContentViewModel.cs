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
        private IEventAggregator _EventAggregator;
        private ISession _Session;
        private DateTime _Now;
        private bool _VisibleListaCooperantes;

        public CooperantesContentViewModel(
            IEventAggregator eventAggregator,
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

            VerListaCooperantesCommand = new DelegateCommand(OnVerListaCooperantesCommand);
            //EditarCitaCommand = new DelegateCommand<CitaWrapper>(OnEditarCitaCommandExecute);

            //SeleccionarPersonaCommand = new DelegateCommand<CitaWrapper>(OnSeleccionarPersonaCommand);

            _EventAggregator.GetEvent<CooperanteCreadoEvent>().Subscribe(OnCooperanteCreadoEvent);
            _EventAggregator.GetEvent<ActividadActualizadaEvent>().Subscribe(OnActividadActualizadaEvent);
            _EventAggregator.GetEvent<ActividadEliminadaEvent>().Subscribe(OnActividadEliminadaEvent);
            _EventAggregator.GetEvent<NuevaActividadEvent>().Subscribe(OnActividadNuevaEvent);
            //_EventAggregator.GetEvent<PersonaActualizadaEvent>().Subscribe(OnPersonaActualizadaEvent);
            //_EventAggregator.GetEvent<AsistenteCreadoEvent>().Subscribe(OnAsistenteCreadoEvent);
            //_EventAggregator.GetEvent<CitaCreadaEvent>().Subscribe(OnCitaCreadaEvent);
            //_EventAggregator.GetEvent<CitaActualizadaEvent>().Subscribe(OnCitaActualizadaEvent);
            SelectActividadCommand = new DelegateCommand<object>(OnSelectActividadCommand);
            EditarActividadCommand = new DelegateCommand<object>(OnEditarActividadCommandExecute);
            BorrarActividadCommand = new DelegateCommand<object>(OnBorrarActividadCommandExecute);
            Gama.Common.Debug.Debug.StopWatch("CooperantesContentViewModel");

        }
        private void OnActividadNuevaEvent(int id)
        {
            var actividad = _ActividadRepository.GetById(id);
            ListaCompletaActividades.Add(actividad);
            RefrescarVista();
            //CooperanteSeleccionado.
            //var actividad = _ActividadRepository.GetById(id);
            //if (actividad.Coordinador.Id == CooperanteSeleccionado.Id) ActividadesCoordina.Add(actividad);
            //var Coop = actividad.Cooperantes.Where(x => x.Id == CooperanteSeleccionado.Id).FirstOrDefault();
            //if (Coop != null) ActividadesCoopera.Add(actividad);
            //var Coop = Cooperantes.Where(x => x.Id == obj.Id).FirstOrDefault();


        }
        private void OnActividadEliminadaEvent(Actividad obj)
        {
            ActividadesCoopera.Remove(obj);
            ActividadesCoordina.Remove(obj);

        }
        private void OnBorrarActividadCommandExecute(object param)
        {
            var lookup = param as Actividad;
            _ActividadRepository.Delete(lookup);
            _EventAggregator.GetEvent<ActividadEliminadaEvent>().Publish(lookup);

        }
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
        private void OnCooperanteCreadoEvent(CooperanteWrapper param)
        {
            //var NewCoop = _CooperanteRepository.GetById(param);
            Cooperantes.Insert(0,param);
        }

        private void OnSelectActividadCommand(object param)
        {
            var lookup = param as Actividad;
            if (lookup != null) _EventAggregator.GetEvent<ActividadSeleccionadaEvent>().Publish(lookup.Id);
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

        private void OnVerListaCooperantesCommand()
        {
            if (VisibleListaCooperantes == true) VisibleListaCooperantes = false;
            else VisibleListaCooperantes = true;
        }

        public ObservableCollection<CooperanteWrapper> Cooperantes { get; private set; }
        public ObservableCollection<Actividad> ListaCompletaActividades { get; private set; }
        public ObservableCollection<Actividad> ActividadesCoordina { get; private set; }
        public ObservableCollection<Actividad> ActividadesCoopera { get; private set; }

        private CooperanteViewModel _CooperanteViewModel;
        public CooperanteViewModel CooperanteViewModel
        {
            get { return _CooperanteViewModel; }
            set { SetProperty(ref _CooperanteViewModel, value); }
        }
        private CooperanteWrapper _CooperanteSeleccionado;
        public CooperanteWrapper CooperanteSeleccionado
        {
            get { return _CooperanteSeleccionado; }
            set
            {
                SetProperty(ref _CooperanteSeleccionado, value);
                RefrescarVista();
                OnPropertyChanged();
            }
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
        public ICommand HabilitarBotonEditarCommand { get; private set; }
        public ICommand GuardarBotonCommand { get; private set; }
        public ICommand CancelarBotonEditarCommand { get; private set; }
        public ICommand EditarCitaCommand { get; private set; }
        public ICommand SeleccionarPersonaCommand { get; private set; }
        public ICommand VerListaCooperantesCommand { get; set; }
        public ICommand SelectActividadCommand { get; set; }
        public ICommand EditarActividadCommand { get; set; }
        public ICommand BorrarActividadCommand { get; set; }
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
                CooperanteSeleccionado = Cooperantes.First();

            OnPropertyChanged(nameof(Cooperantes));
        }
        private void OnGuardarBotonCommand()
        {
            _CooperanteViewModel.Cooperante.UpdatedAt = DateTime.Now;
            _CooperanteRepository.Update(_CooperanteViewModel.Cooperante.Model);
            _CooperanteViewModel.Cooperante.AcceptChanges();
            _CooperanteViewModel.Cooperante.IsInEditionMode = false;
            //RefrescarTitulo(AsistenteSeleccionado.Nombre);
            //if (_CooperanteViewModel.Cooperante._SavedNif != _CooperanteViewModel.Cooperante.Nif)
            //{
            //    //CooperacionResources.TodosLosNifDeAsistentes.Remove(_CooperanteViewModel.Cooperante._SavedNif);
            //    //CooperacionResources.TodosLosNifDeAsistentes.Add(_CooperanteViewModel.Cooperante.Nif);
            //    //CooperanteSeleccionado._SavedNif = _CooperanteViewModel.Cooperante.Nif;
            //}
            //_EventAggregator.GetEvent<AsistenteActualizadoEvent>().Publish(_AsistenteViewModel.Asistente.Id);

        }
        private void OnHabilitarBotonEditarCommand()
        {
            _CooperanteViewModel.Cooperante.IsInEditionMode = true;
        }
        private void OnCancelarEdicionCommand()
        {
            _CooperanteViewModel.Cooperante.RejectChanges();
            _CooperanteViewModel.Cooperante.IsInEditionMode = false;
        }
        private void InvalidateCommands()
        {
            ((DelegateCommand)HabilitarBotonEditarCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)GuardarBotonCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)CancelarBotonEditarCommand).RaiseCanExecuteChanged();
        }
        public bool VisibleListaCooperantes
        {
            get { return _VisibleListaCooperantes; }
            set { SetProperty(ref _VisibleListaCooperantes, value); }
        }// copiado
    }
}
