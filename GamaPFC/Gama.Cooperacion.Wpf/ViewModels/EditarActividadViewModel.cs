using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gama.Cooperacion.Business;
using Prism.Regions;
using Gama.Cooperacion.Wpf.Services;
using Gama.Cooperacion.Wpf.Wrappers;
using NHibernate;
using Prism.Commands;
using System.Windows.Input;
using Prism.Events;
using Gama.Cooperacion.Wpf.Eventos;
using System.ComponentModel;
using Prism;
using Core.Util;
using Gama.Common.Views;

namespace Gama.Cooperacion.Wpf.ViewModels
{
    public class EditarActividadViewModel : ViewModelBase, IActiveAware, IConfirmarPeticionDeNavegacion
    {
        private IEventAggregator _EventAggregator;
        private ICooperanteRepository _CooperanteRepository;
        private IActividadRepository _ActividadRepository;
        private InformacionDeActividadViewModel _InformacionDeActividadViewModel;
        private TareasDeActividadViewModel _TareasDeActividadViewModel;
        private ISession _Session;
        private Preferencias _Preferencias;

        public EditarActividadViewModel(
            IActividadRepository actividadRepository,
            ICooperanteRepository cooperanteRepository,
            IEventAggregator eventAggregator,
            InformacionDeActividadViewModel informacionDeActividadViewModel,
            TareasDeActividadViewModel tareasActividadViewModel,
            ISession session,
            Preferencias preferencias)
        {
            _EventAggregator = eventAggregator;
            _Session = session;
            _ActividadRepository = actividadRepository;
            _CooperanteRepository = cooperanteRepository;
            _InformacionDeActividadViewModel = informacionDeActividadViewModel;
            _TareasDeActividadViewModel = tareasActividadViewModel;
            _Preferencias = preferencias;

            _ActividadRepository.Session = _Session;
            _CooperanteRepository.Session = _Session;
            _TareasDeActividadViewModel.Session = session;
            _InformacionDeActividadViewModel.Session = session;

            _VisibleActividadInfo = false;

            HabilitarEdicionCommand = new DelegateCommand(
                OnHabilitarEdicionCommand,
                () => !_InformacionDeActividadViewModel.Actividad.IsInEditionMode);

            ActualizarCommand = new DelegateCommand(
                OnActualizarCommand,
                () => _InformacionDeActividadViewModel.Actividad.IsInEditionMode
                && _InformacionDeActividadViewModel.Actividad.IsChanged);

            CancelarEdicionCommand = new DelegateCommand(OnCancelarEdicionCommand,
                () => _InformacionDeActividadViewModel.Actividad.IsInEditionMode);
            VerActividadInfoCommand = new DelegateCommand(OnVerActividadInfoCommand);

            _EventAggregator.GetEvent<ActividadActualizadaEvent>().Subscribe(OnActividadActualizadaEvent);

            _InformacionDeActividadViewModel.PropertyChanged += _InvalidateCommands_PropertyChanged;
            Actividad.PropertyChanged += _InvalidateCommands_PropertyChanged;
        }
        private void OnActividadActualizadaEvent(Actividad wrapper)
        {
            ActividadWrapper act = new ActividadWrapper(wrapper);

            //_InformacionDeActividadViewModel.Load(act); // Va a informaciondeActividadVM y cargo los datos de la activdadseleccionada
            //_InformacionDeActividadViewModel.Actividad.IsInEditionMode =
            //    _Preferencias.General_EdicionHabilitadaPorDefecto;
            Actividad.IsInEditionMode = false;
            RefrescarTitulo(Actividad.Titulo);
            InvalidateCommands();
            //_InformacionDeActividadViewModel.PropertyChanged += _InvalidateCommands_PropertyChanged;
            Actividad.PropertyChanged += _InvalidateCommands_PropertyChanged;
            OnPropertyChanged(nameof(InformacionDeActividadViewModel));
        }

        public ICommand HabilitarEdicionCommand { get; private set; }
        public ICommand ActualizarCommand { get; private set; }
        public ICommand CancelarEdicionCommand { get; private set; }
        public ICommand VerActividadInfoCommand { get; set; }

        private bool _VisibleActividadInfo;
        public bool VisibleActividadInfo
        {
            get { return _VisibleActividadInfo; }
            set { SetProperty(ref _VisibleActividadInfo, value); }
        }

        private bool _IsActive;
        public bool IsActive
        {
            get { return _IsActive; }

            set
            {
                SetProperty(ref _IsActive, value);
                if (_IsActive)
                    _EventAggregator.GetEvent<ActividadSeleccionadaChangedEvent>().Publish(Actividad.Id);
            }
        }

        public InformacionDeActividadViewModel InformacionDeActividadViewModel => _InformacionDeActividadViewModel;
        public TareasDeActividadViewModel TareasDeActividadViewModel => _TareasDeActividadViewModel;
        public ActividadWrapper Actividad
        {
            get { return _InformacionDeActividadViewModel.Actividad; }
        }

        private void OnVerActividadInfoCommand()
        {
            if (VisibleActividadInfo == true) VisibleActividadInfo = false;
            else VisibleActividadInfo = true;
        }

        private void OnHabilitarEdicionCommand()
        {
            //Actividad.IsInEditionMode = true;
            _InformacionDeActividadViewModel.Actividad.IsInEditionMode = true;
            if (_InformacionDeActividadViewModel.CooperantesDisponibles.Count > 0
                && Actividad.Cooperantes.Where(c => c.Nombre == "").ToList().Count == 0)
            {
                if (Actividad.Cooperantes.All(c => c.Id != 0))
                    Actividad.Cooperantes.Add(new CooperanteWrapper(new Cooperante()));
                //Actividad.Cooperantes.Add(new CooperanteWrapper(new Cooperante()));
            }
            _InformacionDeActividadViewModel.Actividad.AcceptChanges();
        }

        private void OnActualizarCommand()
        {
            //UIServices.SetBusyState();
            CooperanteWrapper[] posDelete = new CooperanteWrapper[Actividad.Cooperantes.Count];
            int index = 0;
            foreach (var Coop in Actividad.Cooperantes)
            {
                if (Coop.Nombre == "")
                {
                    posDelete[index] = Coop;
                    index++;
                }
                else
                {
                    posDelete[index] = Coop;
                    index++;
                }
            }
            for (int i = 0; i < index; i++)
            {
                if (posDelete[i].Nombre == "") Actividad.Cooperantes.Remove(posDelete[i]);
            }
            //Actividad.UpdatedAt = DateTime.Now;
            _ActividadRepository.Update(Actividad.Model);
            _InformacionDeActividadViewModel.Actividad.AcceptChanges();
            _InformacionDeActividadViewModel.Actividad.IsInEditionMode = false;
            //_ActividadVM.Actividad.Cooperantes.Add(cooperanteDummy);
            _InformacionDeActividadViewModel.Actividad.IsInEditionMode = false;
            RefrescarTitulo(Actividad.Titulo);
            // _EventAggregator.GetEvent<ActividadActualizadaEvent>().Publish(Actividad.Id);
        }

        private void OnCancelarEdicionCommand()
        {
            Actividad.RejectChanges();
            var cooperanteDummy = Actividad.Cooperantes.Where(c => c.Nombre == "").ToList();
            if (cooperanteDummy.Count > 0)
            {
                Actividad.Cooperantes.Remove(cooperanteDummy.First());
            }
            Actividad.AcceptChanges();
            _InformacionDeActividadViewModel.Actividad.IsInEditionMode = false;
        }

        public bool IsNavigationTarget(int id)
        {
            return (Actividad.Id == id);
        }

        public override void OnActualizarServidor()
        {

            ActividadWrapper wrapper = new ActividadWrapper(
                        _ActividadRepository.GetById(Actividad.Id));

            _InformacionDeActividadViewModel.Load(wrapper); // Va a informaciondeActividadVM y cargo los datos de la activdadseleccionada
            _InformacionDeActividadViewModel.Actividad.IsInEditionMode =
                _Preferencias.General_EdicionHabilitadaPorDefecto;
            _TareasDeActividadViewModel.LoadActividad(wrapper);

            RefrescarTitulo(Actividad.Titulo);
            InvalidateCommands();

            _InformacionDeActividadViewModel.PropertyChanged += _InvalidateCommands_PropertyChanged;
            Actividad.PropertyChanged += _InvalidateCommands_PropertyChanged;
            OnPropertyChanged(nameof(InformacionDeActividadViewModel));

        }

        public void OnNavigatedTo(int actividadId) // Se carga la informacion de la actividad con los Load de informaciondeactividadVW y tareasde activdadVM
        {
            try
            {
                if (string.IsNullOrEmpty(Actividad.Titulo)) //Cuando ya esta abierta aqui no entra yno la recarga
                {
                    ActividadWrapper wrapper = new ActividadWrapper(
                        _ActividadRepository.GetById(actividadId));

                    _InformacionDeActividadViewModel.Load(wrapper); // Va a informaciondeActividadVM y cargo los datos de la activdadseleccionada
                    _InformacionDeActividadViewModel.Actividad.IsInEditionMode =
                        _Preferencias.General_EdicionHabilitadaPorDefecto;
                    _TareasDeActividadViewModel.LoadActividad(wrapper);

                    RefrescarTitulo(Actividad.Titulo);
                    InvalidateCommands();
                }

                _InformacionDeActividadViewModel.PropertyChanged += _InvalidateCommands_PropertyChanged;
                Actividad.PropertyChanged += _InvalidateCommands_PropertyChanged;
                OnPropertyChanged(nameof(InformacionDeActividadViewModel));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void _InvalidateCommands_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_InformacionDeActividadViewModel.Actividad.IsInEditionMode))
            {
                InvalidateCommands();
            }
            else if (e.PropertyName == nameof(Actividad))
            {
                Actividad.PropertyChanged += (s, ea) => {
                    if (ea.PropertyName == nameof(Actividad.IsChanged)
                        || ea.PropertyName == nameof(Actividad.IsValid))
                    {
                        InvalidateCommands();
                    }
                };
            }
        }

        private void RefrescarTitulo(string nombre)
        {
            if (nombre.Length > 20)
            {
                Title = nombre.Substring(0, 20) + "...";
            }
            else
            {
                Title = nombre;
            }
        }

        private void InvalidateCommands()
        {
            ((DelegateCommand)HabilitarEdicionCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)ActualizarCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)CancelarEdicionCommand).RaiseCanExecuteChanged();
        }

        public bool ConfirmarPeticionDeNavegacion()
        {
            if (Actividad.IsChanged)
            {
                var o = new ConfirmarOperacionView();
                o.Mensaje = "Si sale se perderán los cambios, ¿Desea salir de todas formas?";
                o.ShowDialog();

                return o.EstaConfirmado;
            }

            return true;
        }

        public void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            throw new NotImplementedException();
        }
    }
}
