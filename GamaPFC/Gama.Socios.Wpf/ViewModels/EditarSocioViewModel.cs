using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using Gama.Socios.Wpf.Services;
using System.Windows.Input;
using Gama.Socios.Wpf.Wrappers;
using Gama.Socios.Wpf.Eventos;
using System.ComponentModel;
using Gama.Common.Views;

namespace Gama.Socios.Wpf.ViewModels
{
    public class EditarSocioViewModel : ViewModelBase
    {
        private EditarCuotasViewModel _CuotasVM;
        private IEventAggregator _EventAggregator;
        private EditarPeriodosDeAltaViewModel _PeriodosDeAltaVM;
        private ISocioRepository _SocioRepository;
        private SocioViewModel _SocioVM;

        public EditarSocioViewModel(
            IEventAggregator eventAggregator,
            ISocioRepository socioRepository,
            SocioViewModel socioVM,
            EditarCuotasViewModel cuotasVM,
            EditarPeriodosDeAltaViewModel periodosDeAltaVM,
            ISession session)
        {
            _EventAggregator = eventAggregator;
            _SocioRepository = socioRepository;
            _SocioVM = socioVM;
            _CuotasVM = cuotasVM;
            _PeriodosDeAltaVM = periodosDeAltaVM;

            _SocioRepository.Session = session;
            _CuotasVM.Session = session;
            _PeriodosDeAltaVM.Session = session;

            HabilitarEdicionCommand = new DelegateCommand(
                OnHabilitarEdicionCommand,
                () => !_SocioVM.EdicionHabilitada);

            ActualizarCommand = new DelegateCommand(
                OnActualizarCommand,
                () => _SocioVM.EdicionHabilitada
                   && Socio.IsChanged
                   && Socio.IsValid);

            CancelarEdicionCommand = new DelegateCommand(OnCancelarEdicionCommand,
                () => _SocioVM.EdicionHabilitada);

            _SocioVM.PropertyChanged += SocioVM_PropertyChanged;
        }

        public SocioWrapper Socio
        {
            get { return _SocioVM.Socio; }
        }

        public SocioViewModel SocioVM
        {
            get { return _SocioVM; }
        }

        public EditarCuotasViewModel CuotasVM
        {
            get { return _CuotasVM; }
        }

        public EditarPeriodosDeAltaViewModel PeriodosDeAltaVM
        {
            get { return _PeriodosDeAltaVM; }
        }

        public ICommand HabilitarEdicionCommand { get; private set; }
        public ICommand ActualizarCommand { get; private set; }
        public ICommand CancelarEdicionCommand { get; private set; }

        private void OnActualizarCommand()
        {
            Socio.UpdatedAt = DateTime.Now;
            _SocioRepository.Update(Socio.Model);
            _SocioVM.Socio.AcceptChanges();
            _SocioVM.EdicionHabilitada = false;
            RefrescarTitulo(Socio.Nombre);
            _EventAggregator.GetEvent<SocioActualizadoEvent>().Publish(this.Socio.Model);
        }

        private void OnHabilitarEdicionCommand()
        {
            _SocioVM.EdicionHabilitada = true;
        }

        private void OnCancelarEdicionCommand()
        {
            Socio.RejectChanges();
            _SocioVM.EdicionHabilitada = false;
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            var id = (int)navigationContext.Parameters["Id"];

            if (Socio.Id == id)
                return true;

            return false;
        }

        public void Load(int id)
        {
            try
            {
                if (this.Socio.Nombre != null)
                    return;

                var Socio = new SocioWrapper(
                    _SocioRepository.GetById(id));

                _SocioVM.Load(Socio);
                _CuotasVM.Load(_SocioVM.Socio);
                _PeriodosDeAltaVM.Load(_SocioVM.Socio);
                RefrescarTitulo(Socio.Nombre);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            var id = (int)navigationContext.Parameters["Id"];
            Load(id);
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

        private void SocioVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_SocioVM.EdicionHabilitada))
            {
                InvalidateCommands();
            }
            else if (e.PropertyName == nameof(Socio))
            {
                Socio.PropertyChanged += (s, ea) => {
                    if (ea.PropertyName == nameof(Socio.IsChanged)
                        || ea.PropertyName == nameof(Socio.IsValid))
                    {
                        InvalidateCommands();
                    }
                };
            }
        }

        public void ConfirmNavigationRequest(NavigationContext navigationContext,
            Action<bool> continuationCallback)
        {
            if (Socio.IsChanged)
            {
                var o = new ConfirmarOperacionView();
                o.Mensaje = "Si sale se perderán los cambios, ¿Desea salir de todas formas?";
                o.ShowDialog();

                continuationCallback.Invoke(o.EstaConfirmado);
            }
        }
    }
}
