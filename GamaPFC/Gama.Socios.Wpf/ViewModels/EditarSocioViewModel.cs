﻿using Core;
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
using Gama.Socios.Business;
using Prism;

namespace Gama.Socios.Wpf.ViewModels
{
    public class EditarSocioViewModel : ViewModelBase, IConfirmNavigationRequest, IActiveAware
    {
        private EditarCuotasViewModel _CuotasVM;
        private IEventAggregator _EventAggregator;
        private EditarPeriodosDeAltaViewModel _EditarPeriodosDeAltaViewModel;
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
            _EditarPeriodosDeAltaViewModel = periodosDeAltaVM;

            _SocioRepository.Session = session;
            _CuotasVM.Session = session;
            _EditarPeriodosDeAltaViewModel.Session = session;

            NuevoPeriodoDeAltaCommand = new DelegateCommand(OnNuevoPeriodoDeAltaCommandExecute);

            HabilitarEdicionCommand = new DelegateCommand(
                OnHabilitarEdicionCommand,
                () => !_SocioVM.Socio.IsInEditionMode);

            ActualizarCommand = new DelegateCommand(
                OnActualizarCommand,
                () => _SocioVM.Socio.IsInEditionMode
                   && Socio.IsChanged
                   && Socio.IsValid);

            CancelarEdicionCommand = new DelegateCommand(OnCancelarEdicionCommand,
                () => _SocioVM.Socio.IsInEditionMode);

            DarDeAltaBajaCommand = new DelegateCommand(OnDarDeAltaBajaCommandExecute);

            _SocioVM.PropertyChanged += SocioVM_PropertyChanged;
        }

        private string _TextoDeDarDeAltaBaja;
        public string TextoDeDarDeAltaBaja
        {
            get { return _TextoDeDarDeAltaBaja; }
            set
            {
                _TextoDeDarDeAltaBaja = value;
                OnPropertyChanged();
            }
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

        public EditarPeriodosDeAltaViewModel EditarPeriodosDeAltaViewModel
        {
            get { return _EditarPeriodosDeAltaViewModel; }
        }

        public ICommand NuevoPeriodoDeAltaCommand { get; private set; }
        public ICommand HabilitarEdicionCommand { get; private set; }
        public ICommand ActualizarCommand { get; private set; }
        public ICommand CancelarEdicionCommand { get; private set; }
        public ICommand DarDeAltaBajaCommand { get; private set; }

        private bool _IsActive;
        public bool IsActive
        {
            get { return _IsActive; }

            set
            {
                SetProperty(ref _IsActive, value);
                if (_IsActive)
                    _EventAggregator.GetEvent<SocioSeleccionadoChangedEvent>().Publish(Socio.Id);
            }
        }
        private void OnNuevoPeriodoDeAltaCommandExecute()
        {
            _EditarPeriodosDeAltaViewModel.AddPeriodoDeAlta();
        }

        private void OnActualizarCommand()
        {
            _SocioRepository.Update(Socio.Model);
            _SocioVM.Socio.AcceptChanges();
            _SocioVM.Socio.IsInEditionMode = false;
            _SocioVM.Socio.IsInEditionMode = false;
            RefrescarTitulo(Socio.Nombre);
        }

        private void OnHabilitarEdicionCommand()
        {
            _SocioVM.Socio.IsInEditionMode = true;
        }

        private void OnCancelarEdicionCommand()
        {
            Socio.RejectChanges();
            _SocioVM.Socio.IsInEditionMode = false;
        }

        private void OnDarDeAltaBajaCommandExecute()
        {
            if (Socio.EstaDadoDeAlta)
            {
                Socio.EstaDadoDeAlta = false;
                TextoDeDarDeAltaBaja = "Dar de alta";
            }
            else
            {
                Socio.EstaDadoDeAlta = true;
                TextoDeDarDeAltaBaja = "Dar de baja";
            }

            _EventAggregator.GetEvent<SocioActualizadoEvent>().Publish(Socio.Model);
        }

        public bool IsNavigationTarget(int id)
        {
            return (Socio.Id == id);
        }

        public override void OnActualizarServidor()
        {
            //if (!Persona.IsChanged)
            //{
            //    var persona = new PersonaWrapper(
            //        (Persona)
            //        _PersonaRepository.GetById(Persona.Id)
            //        .DecryptFluent());

            //    _PersonaVM.Load(persona);
            //    _AtencionesVM.Load(_PersonaVM.Persona);
            //    _CitasVM.Load(_PersonaVM.Persona);
            //    RefrescarTitulo(persona.Nombre);
            //    _AtencionesVM.VerAtenciones = false;
            //}
        }

        public void OnNavigatedTo(int id)
        {
            try
            {

                //if (this.Socio.Nombre != "")
                //    return;
                if (string.IsNullOrEmpty(this.Socio.Nombre))
                {
                    var Socio = new SocioWrapper(
                   _SocioRepository.GetById(id));

                    _SocioVM.Load(Socio);
                    // _CuotasVM.Load(_SocioVM.Socio);
                    _EditarPeriodosDeAltaViewModel.Load(_SocioVM.Socio);
                    RefrescarTitulo(Socio.Nombre);
                    TextoDeDarDeAltaBaja = Socio.EstaDadoDeAlta ? "Dar de baja" : "Dar de alta";
                }
                else return;
            }
            catch (Exception)
            {
                throw;
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

        private void SocioVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_SocioVM.Socio.IsInEditionMode))
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

        public bool ConfirmNavigationRequest()
        {
            if (Socio.IsChanged)
            {
                var o = new ConfirmarOperacionView();
                o.Mensaje = "Si sale se perderán los cambios, ¿Desea salir de todas formas?";
                o.ShowDialog();

                return o.EstaConfirmado;
            }

            return true;
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
