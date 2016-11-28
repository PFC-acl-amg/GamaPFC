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

namespace Gama.Cooperacion.Wpf.ViewModels
{
    public class EditarActividadViewModel : ViewModelBase
    {
        private IActividadRepository _ActividadRepository;
        private InformacionDeActividadViewModel _ActividadVM;
        private TareasDeActividadViewModel _TareasActividadVM;
        private ICooperanteRepository _CooperanteRepository;
        private ISession _Session;
        private IEventAggregator _eventAggregator;

        public EditarActividadViewModel(
            IActividadRepository actividadRepository,
            ICooperanteRepository cooperanteRepository,
            IEventAggregator eventAggregator,
            InformacionDeActividadViewModel actividadVM,
            TareasDeActividadViewModel tareasActividadVM,
            ISession session)
        {
            _eventAggregator = eventAggregator;
            _ActividadRepository = actividadRepository;
            _CooperanteRepository = cooperanteRepository;
            _ActividadRepository.Session = session;
            _CooperanteRepository.Session = session;
            _Session = session;
            _ActividadVM = actividadVM;
            _TareasActividadVM = tareasActividadVM;

            HabilitarEdicionCommand = new DelegateCommand(
                OnHabilitarEdicionCommand,
                () => !_ActividadVM.EdicionHabilitada);

            ActualizarCommand = new DelegateCommand(
                OnActualizarCommand,
                () => _ActividadVM.EdicionHabilitada && _ActividadVM.Actividad.IsChanged);

            CancelarEdicionCommand = new DelegateCommand(OnCancelarEdicionCommand,
                () => _ActividadVM.EdicionHabilitada);

            _ActividadVM.PropertyChanged += _ActividadVM_PropertyChanged;
        }

        public InformacionDeActividadViewModel ActividadVM
        {
            get { return _ActividadVM; }
        }
        public TareasDeActividadViewModel TareasActividadVM
        {
            get { return _TareasActividadVM; }
        }
        public ActividadWrapper Actividad
        {
            get { return _ActividadVM.Actividad; }
        }

        public ICommand HabilitarEdicionCommand { get; private set; }
        public ICommand ActualizarCommand { get; private set; }
        public ICommand CancelarEdicionCommand { get; private set; }

        private void OnHabilitarEdicionCommand()
        {
            _ActividadVM.EdicionHabilitada = true;
            if (_ActividadVM.CooperantesDisponibles.Count > 0
                && Actividad.Cooperantes.Where(c => c.Nombre == null).ToList().Count == 0)
            {
                Actividad.Cooperantes.Add(new CooperanteWrapper(new Cooperante()));
            }
            _ActividadVM.Actividad.AcceptChanges();
        }

        private void OnActualizarCommand()
        {
            var cooperanteDummy = Actividad.Cooperantes.Single(c => c.Nombre == null);
            if (cooperanteDummy != null)
            {
                Actividad.Cooperantes.Remove(cooperanteDummy);
            }
            Actividad.UpdatedAt = DateTime.Now;
            _ActividadRepository.Update(Actividad.Model);
            _ActividadVM.Actividad.AcceptChanges();
            //_ActividadVM.Actividad.Cooperantes.Add(cooperanteDummy);
            _ActividadVM.EdicionHabilitada = false;
            _eventAggregator.GetEvent<ActividadActualizadaEvent>().Publish(Actividad.Id);
        }

        private void OnCancelarEdicionCommand()
        {
            Actividad.RejectChanges();
            var cooperanteDummy = Actividad.Cooperantes.Where(c => c.Nombre == null).ToList();
            if (cooperanteDummy.Count > 0)
            {
                Actividad.Cooperantes.Remove(cooperanteDummy.First());
            }
            Actividad.AcceptChanges();
            _ActividadVM.EdicionHabilitada = false;
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            var id = (int)navigationContext.Parameters["Id"];

            if (Actividad.Id == id)
                return true;

            return false;
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            var actividad = new ActividadWrapper(
                _ActividadRepository.GetById((int)navigationContext.Parameters["Id"]));

            _ActividadVM.Load(actividad);
            _TareasActividadVM.LoadActividad(actividad);

            if (Actividad.Titulo.Length > 20)
            {
                Title = Actividad.Titulo.Substring(0, 20);
            } 
            else
            {
                Title = Actividad.Titulo;
            }
        }

        private void InvalidateCommands()
        {
            ((DelegateCommand)HabilitarEdicionCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)ActualizarCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)CancelarEdicionCommand).RaiseCanExecuteChanged();
        }

        private void _ActividadVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_ActividadVM.EdicionHabilitada))
            {
                InvalidateCommands();
            }
            else if (e.PropertyName == nameof(Actividad))
            {
                Actividad.PropertyChanged += (s, ea) => {
                    InvalidateCommands();
                };
            }
        }
    }
}
