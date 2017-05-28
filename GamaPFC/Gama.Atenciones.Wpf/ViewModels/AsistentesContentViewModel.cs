﻿using Core;
using Gama.Atenciones.Business;
using Gama.Atenciones.Wpf.Eventos;
using Gama.Atenciones.Wpf.Services;
using Gama.Atenciones.Wpf.Views;
using Gama.Atenciones.Wpf.Wrappers;
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

namespace Gama.Atenciones.Wpf.ViewModels
{
    public class AsistentesContentViewModel : ViewModelBase
    {
        private IAsistenteRepository _AsistenteRepository;
        private ICitaRepository _CitaRepository;
        private IEventAggregator _EventAggregator;
        private ISession _Session;

        public AsistentesContentViewModel(
            IEventAggregator eventAggregator,
            IAsistenteRepository asistenteRepository,
            ICitaRepository citaRepository,
            AsistenteViewModel asistenteViewModel,
            ISession session)
        {
            _EventAggregator = eventAggregator;
            _AsistenteRepository = asistenteRepository;
            _CitaRepository = citaRepository;
            _AsistenteRepository.Session = session;
            _CitaRepository.Session = session;
            _AsistenteViewModel = asistenteViewModel;
            _Session = session;

            AsistenteSeleccionado = new AsistenteWrapper(new Business.Asistente());

            Asistentes = new ObservableCollection<AsistenteWrapper>(
                _AsistenteRepository.GetAll()
                .Select(x => new AsistenteWrapper(x)));

            if (Asistentes.Count > 0)
                AsistenteSeleccionado = Asistentes.First();

            HabilitarEdicionCommand = new DelegateCommand(
                OnHabilitarEdicionCommand,
                () => !_AsistenteViewModel.Asistente.IsInEditionMode);

            ActualizarCommand = new DelegateCommand(
                OnActualizarCommand,
                () =>
                _AsistenteViewModel.Asistente.IsInEditionMode
                   && _AsistenteViewModel.Asistente.IsChanged
                   && _AsistenteViewModel.Asistente.IsValid
                   );

            CancelarEdicionCommand = new DelegateCommand(OnCancelarEdicionCommand,
                () => _AsistenteViewModel.Asistente.IsInEditionMode);

            EditarCitaCommand = new DelegateCommand<CitaWrapper>(OnEditarCitaCommandExecute);

            SeleccionarPersonaCommand = new DelegateCommand<CitaWrapper>(OnSeleccionarPersonaCommand);

            _EventAggregator.GetEvent<AsistenteCreadoEvent>().Subscribe(OnAsistenteCreadoEvent);
            _EventAggregator.GetEvent<CitaCreadaEvent>().Subscribe(OnCitaCreadaEvent);
            _EventAggregator.GetEvent<CitaActualizadaEvent>().Subscribe(OnCitaActualizadaEvent);
        }

        private void OnSeleccionarPersonaCommand(CitaWrapper wrapper)
        {
            _EventAggregator.GetEvent<PersonaSeleccionadaEvent>().Publish(wrapper.Persona.Id);
        }

        public ObservableCollection<AsistenteWrapper> Asistentes { get; private set; }

        private AsistenteViewModel _AsistenteViewModel;
        public AsistenteViewModel AsistenteViewModel
        {
            get { return _AsistenteViewModel; }
            set { SetProperty(ref _AsistenteViewModel, value); }
        }

        private AsistenteWrapper _AsistenteSeleccionado;
        public AsistenteWrapper AsistenteSeleccionado
        {
            get { return _AsistenteSeleccionado; }
            set
            {
                SetProperty(ref _AsistenteSeleccionado, value);
                if (_AsistenteSeleccionado != null)
                {
                    _AsistenteViewModel.Load(_AsistenteSeleccionado);
                }
                else
                {
                    _AsistenteSeleccionado = new AsistenteWrapper(new Business.Asistente());
                    _AsistenteViewModel.Load(_AsistenteSeleccionado);
                }

                _AsistenteSeleccionado.PropertyChanged += (s, e) => InvalidateCommands();
                OnPropertyChanged();
            }
        }

        public ICommand HabilitarEdicionCommand { get; private set; }
        public ICommand ActualizarCommand { get; private set; }
        public ICommand CancelarEdicionCommand { get; private set; }
        public ICommand EditarCitaCommand { get; private set; }
        public ICommand SeleccionarPersonaCommand { get; private set; }

        private void OnActualizarCommand()
        {
            _AsistenteViewModel.Asistente.UpdatedAt = DateTime.Now;
            _AsistenteRepository.Update(_AsistenteViewModel.Asistente.Model);
            _AsistenteViewModel.Asistente.AcceptChanges();
            _AsistenteViewModel.Asistente.IsInEditionMode = false;
            //RefrescarTitulo(AsistenteSeleccionado.Nombre);
            if (_AsistenteViewModel.Asistente._SavedNif != _AsistenteViewModel.Asistente.Nif)
            {
                AtencionesResources.TodosLosNifDeAsistentes.Remove(_AsistenteViewModel.Asistente._SavedNif);
                AtencionesResources.TodosLosNifDeAsistentes.Add(_AsistenteViewModel.Asistente.Nif);
                AsistenteSeleccionado._SavedNif = _AsistenteViewModel.Asistente.Nif;
            }
            _EventAggregator.GetEvent<AsistenteActualizadoEvent>().Publish(_AsistenteViewModel.Asistente.Id);
        }

        private void OnHabilitarEdicionCommand()
        {
            _AsistenteViewModel.Asistente.IsInEditionMode = true;
        }

        private void OnCancelarEdicionCommand()
        {
            _AsistenteViewModel.Asistente.RejectChanges();
            _AsistenteViewModel.Asistente.IsInEditionMode = false;
        }

        private void OnEditarCitaCommandExecute(CitaWrapper cita)
        {
            var o = new NuevaCitaView() { Title = "Editar Cita" };
            var vm = (NuevaCitaViewModel)o.DataContext;
            vm.Session = _Session;
            vm.EnEdicionDeCitaExistente = true;
            vm.LoadForEdition(cita);
            o.ShowDialog();
            cita.AcceptChanges();
        }

        private void OnAsistenteCreadoEvent(int id)
        {
            var asistente = _AsistenteRepository.GetById(id);
            var wrapper = new AsistenteWrapper(asistente);

            Asistentes.Insert(0, wrapper);
            AsistenteSeleccionado = wrapper;
        }

        private void OnCitaCreadaEvent(int id)
        {
            Cita cita = _CitaRepository.GetById(id);
            AsistenteWrapper asistente = Asistentes.First(x => x.Id == cita.Asistente.Id);

            asistente.Citas.Add(cita);

            // Refresca la vista, realmente no sé por qué hace falta reseleccionarlo. 
            // Si no se pone esta línea, al deseleccionar y volver a seleccionar al asistente
            // afectado, la vista se refresca con la nueva cita.
            AsistenteSeleccionado = AsistenteSeleccionado; 
        }

        private void OnCitaActualizadaEvent(int id)
        {
            Cita cita = _CitaRepository.GetById(id);
            AsistenteWrapper asistente = Asistentes.First(x => x.Id == cita.Asistente.Id);
            Cita citaDesactualizada = asistente.Citas.First(x => x.Id == id);

            citaDesactualizada.CopyValuesFrom(cita);
            OnPropertyChanged(nameof(AsistenteSeleccionado));
        }

        private void InvalidateCommands()
        {
            ((DelegateCommand)HabilitarEdicionCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)ActualizarCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)CancelarEdicionCommand).RaiseCanExecuteChanged();
        }
    }
}
