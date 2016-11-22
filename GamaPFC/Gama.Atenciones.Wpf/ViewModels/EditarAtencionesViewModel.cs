﻿using Core;
using Gama.Atenciones.Business;
using Gama.Atenciones.Wpf.Eventos;
using Gama.Atenciones.Wpf.Services;
using Gama.Atenciones.Wpf.Wrappers;
using NHibernate;
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

namespace Gama.Atenciones.Wpf.ViewModels
{
    public class EditarAtencionesViewModel : ViewModelBase
    {
        private bool _EdicionHabilitada;
        private PersonaWrapper _Persona;
        private AtencionWrapper _AtencionSeleccionada;
        private IAtencionRepository _AtencionRepository;
        private ISession _Session;
        private IEventAggregator _EventAggregator;
        private IPersonaRepository _PersonaRepository;
        private IRegionManager _RegionManager;

        public EditarAtencionesViewModel(IAtencionRepository atencionRepository,
            IEventAggregator eventAggregator, IPersonaRepository personaRepository,
            IRegionManager regionManager)
        {
            _EdicionHabilitada = true;
            Persona = new PersonaWrapper(new Persona());
            _AtencionRepository = atencionRepository;
            _PersonaRepository = personaRepository;
            _EventAggregator = eventAggregator;
            _RegionManager = regionManager;

            HabilitarEdicionCommand = new DelegateCommand(
                OnHabilitarEdicionCommand,
                () => !EdicionHabilitada);

            ActualizarCommand = new DelegateCommand(
                OnActualizarCommand,
                () => EdicionHabilitada
                   && AtencionSeleccionada != null
                   && AtencionSeleccionada.IsChanged
                   && AtencionSeleccionada.IsValid);

            CancelarEdicionCommand = new DelegateCommand(OnCancelarEdicionCommand,
                () => EdicionHabilitada);

            PropertyChanged += EditarAtencionesViewModel_PropertyChanged;

            _EventAggregator.GetEvent<NuevaAtencionEvent>().Subscribe(OnNuevaAtencionEvent);
        }

        public void Load(PersonaWrapper wrapper)
        {
            EdicionHabilitada = false;
            Persona = wrapper;
            Atenciones = new ObservableCollection<AtencionWrapper>(
                Persona.Citas.Select(c => c.Atencion).Where(a => a != null && a.Id != 0).ToList());
            //OnPropertyChanged("Atenciones");
        }

        private void EditarAtencionesViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AtencionSeleccionada)
                || e.PropertyName == nameof(EdicionHabilitada))
            {
                InvalidateCommands();
            }
        }

        private void InvalidateCommands()
        {
            ((DelegateCommand)HabilitarEdicionCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)ActualizarCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)CancelarEdicionCommand).RaiseCanExecuteChanged();
        }

        private void OnNuevaAtencionEvent(CitaWrapper cita)
        {
            if (cita.Atencion == null)
            {
                var atencion = new AtencionWrapper(new Atencion());
                atencion.Model.Cita = cita.Model;
                Atenciones.Add(atencion);
                AtencionSeleccionada = atencion;
            }
            else // ya existe
            {
                AtencionSeleccionada = cita.Atencion;
            }

            VerAtenciones = true;
            //_RegionManager.Regions["AtencionesMasterDetailRegion"].Activate(_RegionManager.Regions["AtencionesMasterDetailRegion"].Views.First());
        }

        private void OnCancelarEdicionCommand()
        {
            AtencionSeleccionada.RejectChanges();
            EdicionHabilitada = false;
        }

        private void OnActualizarCommand()
        {
            AtencionSeleccionada.UpdatedAt = DateTime.Now;
            _AtencionRepository.Update(AtencionSeleccionada.Model);
            AtencionSeleccionada.AcceptChanges();
            EdicionHabilitada = false;
        }

        private void OnHabilitarEdicionCommand()
        {
            EdicionHabilitada = true;
        }

        public ICommand HabilitarEdicionCommand { get; private set; }
        public ICommand ActualizarCommand { get; private set; }
        public ICommand CancelarEdicionCommand { get; private set; }

        public ISession Session
        {
            get { return _Session; }
            set
            {
                _Session = value;
                _AtencionRepository.Session = _Session;
                _PersonaRepository.Session = _Session;
            }
        }

        private bool _VerAtenciones = false;
        public bool VerAtenciones
        {
            get { return _VerAtenciones; }
            set { SetProperty(ref _VerAtenciones, value); }
        }

        public PersonaWrapper Persona
        {
            get { return _Persona; }
            set { SetProperty(ref _Persona, value); }
        }

        public ObservableCollection<AtencionWrapper> Atenciones { get; private set; }

        public AtencionWrapper AtencionSeleccionada
        {
            get { return _AtencionSeleccionada; }
            set { SetProperty(ref _AtencionSeleccionada, value); }
        }

        public bool EdicionHabilitada
        {
            get { return _EdicionHabilitada; }
            set { SetProperty(ref _EdicionHabilitada, value); }
        }
    }
}
