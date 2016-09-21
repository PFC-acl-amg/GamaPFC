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

namespace Gama.Cooperacion.Wpf.ViewModels
{
    public class ActividadDetailViewModel : ViewModelBase
    {
        private IActividadRepository _ActividadRepository;
        private InformacionDeActividadViewModel _ActividadVM;
        private ICooperanteRepository _CooperanteRepository;
        private ISession _Session;

        public ActividadWrapper Actividad { get; set; }

        public ActividadDetailViewModel(IActividadRepository actividadRepository,
            ICooperanteRepository cooperanteRepository,
            InformacionDeActividadViewModel actividadVM, ISession session)
        {
            _ActividadRepository = actividadRepository;
            _CooperanteRepository = cooperanteRepository;
            _ActividadRepository.Session = session;
            _CooperanteRepository.Session = session;
            _Session = session;
            _ActividadVM = actividadVM;

            HabilitarEdicionCommand = new DelegateCommand(
                () => _ActividadVM.EdicionHabilitada = true,
                () => !_ActividadVM.EdicionHabilitada);

            GuardarInformacionCommand = new DelegateCommand(OnGuardarInformacion,
                () => _ActividadVM.EdicionHabilitada && _ActividadVM.Actividad.IsChanged);

            _ActividadVM.PropertyChanged += _ActividadVM_PropertyChanged;
        }

        private void _ActividadVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_ActividadVM.EdicionHabilitada))
            {
                InvalidateCommands();
            }
            else if (e.PropertyName == nameof(_ActividadVM.Actividad))
            {
                _ActividadVM.Actividad.PropertyChanged += Actividad_PropertyChanged;
            }
        }

        private void Actividad_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_ActividadVM.Actividad.IsChanged))
            {
                InvalidateCommands();
            }
        }

        public InformacionDeActividadViewModel ActividadVM
        {
            get { return _ActividadVM; }
        }

        public ICommand HabilitarEdicionCommand { get; private set; }
        public ICommand GuardarInformacionCommand { get; private set; }

        private void OnGuardarInformacion()
        {
            //_CooperanteRepository.Update(Actividad.Model.Coordinador);
            //_ActividadRepository.Update(Actividad.Model);
            var cooperanteDummy = _ActividadVM.Actividad.Cooperantes.Where(c => c.Nombre == null).First();
            if (cooperanteDummy != null)
            {
                _ActividadVM.Actividad.Cooperantes.Remove(cooperanteDummy);
            }
            _ActividadRepository.Flush();
            _ActividadVM.Actividad.AcceptChanges();
            _ActividadVM.Actividad.Cooperantes.Add(cooperanteDummy);
            _ActividadVM.EdicionHabilitada = false;
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            var id = (int)navigationContext.Parameters["Id"];

            if (Actividad != null && Actividad.Id == id)
                return true;

            return false;
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            Actividad = new ActividadWrapper(
                _ActividadRepository.GetById((int)navigationContext.Parameters["Id"]));
            //Actividad.Coordinador = new CooperanteWrapper(
            //    _CooperanteRepository.GetById(Actividad.CoordinadorId));

            _ActividadVM.InicializarParaVer(Actividad);

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
            ((DelegateCommand)GuardarInformacionCommand).RaiseCanExecuteChanged();
        }
    }
}
